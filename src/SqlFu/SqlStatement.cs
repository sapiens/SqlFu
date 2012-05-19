using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CavemanTools.Model;

namespace SqlFu
{
    public interface IQuerySqlStatement
    {
        int Execute();
        T ExecuteScalar<T>(Func<object, T> converter=null);
        
        IEnumerable<T> ExecuteQuery<T>(Func<IDataReader, T> mapper=null);
    }

    public class SqlStatement:IDisposable, IQuerySqlStatement
    {
        protected DbAccess _db;
        protected IDbCommand _cmd;
        
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
                _paramNames = paramDict.Keys.ToArray();
                var sb = new StringBuilder();
                var lastParamCount = args.Length;
                
                IDbDataParameter p=null;
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
                            provider.SetupParameter(p, lastParamCount.ToString(), val);
                            allp.Add(p);
                            lastParamCount++;
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sql=sql.Replace("@" + kv.Key, sb.ToString());
                    }
                    else
                    {
                        p = _cmd.CreateParameter();
                        provider.SetupParameter(p, kv.Key, kv.Value);
                        allp.Add(p);
                    }
                }
            }
            else
            {
                _paramNames = new string[0];
            }
            _cmd.CommandText = sql;
        }

        IDictionary<string, object> CreateParamsDictionary(params object[] args)
        {
            IDictionary<string, object> d;

            if (args.Length == 1)
            {
                var poco = args[0];
                if (poco != null && !poco.IsListParam())
                {
                    
                    if (Type.GetTypeCode(poco.GetType()) == TypeCode.Object)
                    {
                        d = poco.ToDictionary();
                        return d;
                    }
                }
            }


            d = new Dictionary<string, object>(args.Length);
            for (int i = 0; i < args.Length; i++)
            {
                d[i.ToString()] = args[i];
            }
            return d;
        }

        internal bool ReuseCommand { get; set; }
        #region Command
        internal void ResetCommand()
        {
            _cmd.Parameters.Clear();
        }

        internal IDbCommand Command
        {
            get { return _cmd; }
        }

        private Action<IDbCommand> _before;

        public IQuerySqlStatement ApplyToCommand(Action<IDbCommand> action)
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

        public T ExecuteScalar<T>(Func<object, T> converter=null)
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
                _db.CloseConnection();
                if (converter == null) converter = PocoFactory.GetConverter<T>();
                return converter(rez);
            }
        }

        public IEnumerable<T> ExecuteQuery<T>(Func<IDataReader, T> mapper=null)
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
                            if (mapper == null) mapper = PocoFactory.GetPocoMapper<T>(rd,_cmd.CommandText);
                            d = mapper(rd);
                        }
                        catch (Exception ex)
                        {
                            Db.OnException(this, ex);
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
            if (_cmd!=null)
            {
                _cmd.Dispose();
                _db.CloseConnection();
                _db = null;
            }
        }
    }
}