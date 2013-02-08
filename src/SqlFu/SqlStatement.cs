using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace SqlFu
{
    public interface IQuerySqlStatement
    {
        int Execute();
        T ExecuteScalar<T>(Func<object, T> converter = null);

        IEnumerable<T> ExecuteQuery<T>(Func<IDataReader, T> mapper = null);
        T QuerySingle<T>(Func<IDataReader, T> mapper = null);
    }

    public interface ISqlStatement : IQuerySqlStatement, IDisposable
    {
        DbAccess Db { get; }

        /// <summary>
        /// Returns last executed sql with params
        /// </summary>
        string ExecutedSql { get; }

        // void SetSql(string sql, params object[] args);
        IQuerySqlStatement ApplyToCommand(Action<DbCommand> action);
    }


    public class SqlStatement : ISqlStatement
    {
        protected DbAccess _db;
        protected DbCommand _cmd;

        public SqlStatement(DbAccess db)
        {
            db.MustNotBeNull();
            _db = db;
            _cmd = _db.CreateCommand();
        }

        public DbAccess Db
        {
            get { return _db; }
        }

        protected string[] _paramNames;

        //public void SetSql(string sql, params  object[] args)
        //{
        //    _cmd.CommandText = sql;

        //    if (args.Length == 0)
        //    {
        //        _paramNames = new string[0];                
        //        return;
        //    }

        //    if (args.Length == 1)
        //    {
        //        var poco = args[0];
        //        if (poco != null)
        //        {
        //            var tp = poco.GetType();
        //            if (tp.IsCustomObjectType())
        //            {

        //                if (!tp.IsListParam())
        //                {
        //                    var mapper = PocoFactory.GetParametersMapper(tp);
        //                    _paramNames = mapper(_cmd, _db.Provider, poco);   
        //                }

        //                return;
        //            }
        //        }
        //    }

        //    _paramNames = new string[args.Length];
        //    var prov = _db.Provider;
        //    IDbDataParameter p;
        //    string pname;
        //    var allp = _cmd.Parameters;

        //    for (int i = 0; i < args.Length; i++)
        //    {
        //        p = _cmd.CreateParameter();
        //        pname = i.ToString();

        //        prov.SetupParameter(p, pname, args[i]);
        //        _paramNames[i] = pname;
        //        allp.Add(p);
        //    }

        //}


        public void SetSql(string sql, params object[] args)
        {
            if (args.Length > 0)
            {
                var provider = _db.Provider;
                var paramDict = CreateParamsDictionary(args);
                var allp = _cmd.Parameters;
                List<string> pnames = new List<string>();
                var sb = new StringBuilder();
                var lastParamCount = args.Length;

                IDbDataParameter p = null;
                foreach (var kv in paramDict)
                {
                    if (kv.Value.IsListParam())
                    {
                        var lp = kv.Value as IEnumerable;

                        sb.Clear();

                        foreach (var val in lp)
                        {
                            p = _cmd.CreateParameter();

                            sb.Append("@" + lastParamCount + ",");
                            pnames.Add(lastParamCount.ToString());
                            provider.SetupParameter(p, lastParamCount.ToString(), val);
                            allp.Add(p);
                            lastParamCount++;
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sql = sql.Replace("@" + kv.Key, sb.ToString());
                    }
                    else
                    {
                        p = _cmd.CreateParameter();
                        provider.SetupParameter(p, kv.Key, kv.Value);
                        pnames.Add(kv.Key);
                        allp.Add(p);
                    }
                }

                _paramNames = pnames.ToArray();
            }
            else
            {
                _paramNames = new string[0];
            }

            _cmd.CommandText = sql;
        }

        protected virtual IDictionary<string, object> CreateParamsDictionary(params object[] args)
        {
            IDictionary<string, object> d = new Dictionary<string, object>();
            if (args != null)
            {
                if (args.Length == 1)
                {
                    var poco = args[0];
                    if (poco != null && !poco.IsListParam())
                    {
                        if (poco.GetType().IsCustomObjectType())
                        {
                            d = poco.ToDictionary();
                            return d;
                        }
                    }
                }

                for (int i = 0; i < args.Length; i++)
                {
                    d.Add(i.ToString(),args[i]);                    
                }
            }
            return d;
        }

        internal bool ReuseCommand { get; set; }

        #region Command

        internal void ResetCommand()
        {
            _cmd.Parameters.Clear();
        }

        internal DbCommand Command
        {
            get { return _cmd; }
        }

        private Action<DbCommand> _before;

        public IQuerySqlStatement ApplyToCommand(Action<DbCommand> action)
        {
            action.MustNotBeNull();
            _before = action;
            return this;
        }

        protected void FormatCommand()
        {
            if (_before != null) _before(_cmd);
            _cmd.CommandText = _db.Provider.FormatSql(_cmd.CommandText, _paramNames);
            _db.Provider.OnCommandExecuting(_cmd);
        }

        #endregion

        #region ExecuteCommand

        internal object ExecuteScalar()
        {
            FormatCommand();
            object rez;
            try
            {
                rez = _cmd.ExecuteScalar();
                Db.OnCommand(_cmd);
            }
            catch (Exception ex)
            {
                Db.OnException(this, ex);
                _cmd.Dispose();
                Db.CloseConnection();
                throw;
            }
            finally
            {
                if (!ReuseCommand)
                {
                    _cmd.Dispose();
                }
            }
            return rez;
        }

        internal IDataReader GetReader()
        {
            FormatCommand();

            try
            {
                var rd = _cmd.ExecuteReader();
                Db.OnCommand(_cmd);
                return rd;
            }
            catch (Exception ex)
            {
                Db.OnException(this, ex);
                Db.CloseConnection();
                throw;
            }
        }


        public int Execute()
        {
            var rez = 0;
            try
            {
                FormatCommand();
                try
                {
                    rez = _cmd.ExecuteNonQuery();
                    Db.OnCommand(_cmd);
                }
                catch (Exception ex)
                {
                    Db.OnException(this, ex);
                    Db.CloseConnection();
                    _cmd.Dispose();
                    throw;
                }
            }
            finally
            {
                if (!ReuseCommand)
                {
                    _cmd.Dispose();
                    _db.CloseConnection();
                }
            }
            return rez;
        }

        public T ExecuteScalar<T>(Func<object, T> converter = null)
        {
            using (_cmd)
            {
                FormatCommand();
                object rez;
                try
                {
                    rez = _cmd.ExecuteScalar();
                    Db.OnCommand(_cmd);
                }
                catch (Exception ex)
                {
                    Db.OnException(this, ex);
                    throw;
                }
                finally
                {
                    _db.CloseConnection();
                }

                if (converter == null) converter = PocoFactory.GetConverter<T>();
                return converter(rez);
            }
        }

        public T QuerySingle<T>(Func<IDataReader, T> mapper = null)
        {
            T d=default(T);
            using (_cmd)
            {
                using (var rd = GetReader())
                {
                    
                    try
                    {
                        if (rd.Read())
                        {
                            if (mapper == null) mapper = PocoFactory.GetPocoMapper<T>(rd, _cmd.CommandText);
                            d = mapper(rd);
                        }
                    }
                    catch (Exception ex)
                    {
                        Db.OnException(this, ex);
                        Db.CloseConnection();
                        throw;
                    }
                    Db.CloseConnection();
                }
            }
            return d;
        }

        public IEnumerable<T> ExecuteQuery<T>(Func<IDataReader, T> mapper = null)
        {
            using (_cmd)
            {
                using (var rd = GetReader())
                {
                    T d;
                    while (true)
                    {
                        try
                        {
                            if (!rd.Read())
                            {
                                break;
                            }
                            if (mapper == null) mapper = PocoFactory.GetPocoMapper<T>(rd, _cmd.CommandText);
                            d = mapper(rd);
                        }
                        catch (Exception ex)
                        {
                            Db.OnException(this, ex);
                            Db.CloseConnection();
                            throw;
                        }

                        yield return d;
                    }
                    Db.CloseConnection();
                }
            }
        }

        #endregion

        /// <summary>
        /// Returns last executed sql with params
        /// </summary>
        public string ExecutedSql
        {
            get
            {
                if (_cmd != null)
                {
                    return _cmd.FormatCommand();
                }
                return "";
            }
        }

        internal string Sql
        {
            get { return _cmd.CommandText; }
            set { _cmd.CommandText = value; }
        }

        public void Dispose()
        {
            if (_cmd != null)
            {
                _cmd.Dispose();
                _db.CloseConnection();
                _db = null;
            }
        }
    }
}