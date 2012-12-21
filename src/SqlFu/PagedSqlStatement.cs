using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using CavemanTools.Model;

namespace SqlFu
{
    public interface IPagedQueryStatement : IQuerySqlStatement
    {
        ResultSet<T> ExecutePagedQuery<T>(Func<IDataReader, T> mapper = null);
    }

    public interface IPagedSqlStatement : IPagedQueryStatement, IDisposable
    {
        DbAccess Db { get; }

        /// <summary>
        /// Returns last executed sql with params
        /// </summary>
        string ExecutedSql { get; }

        // void SetSql(string sql, params object[] args);
        IPagedQueryStatement ApplyToCommand(Action<DbCommand> action);
    }

    public class PagedSqlStatement : SqlStatement, IPagedSqlStatement
    {
        public const string SkipParameterName = "_fuskip";
        public const string TakeParameterName = "_futake";
        private long _skip;

        private int _take
                    ;

        public PagedSqlStatement(DbAccess db) : base(db)
        {
        }

        public void SetSql(long skip, int take, string sql, params object[] args)
        {
            SetSql(sql, args);

            _skip = skip;
            _take = take;
        }

        private void AddPagingParams()
        {
            var sp = _cmd.CreateParameter();
            Db.Provider.SetupParameter(sp, SkipParameterName, _skip);
            _cmd.Parameters.Add(sp);

            var tp = _cmd.CreateParameter();
            Db.Provider.SetupParameter(tp, TakeParameterName, _take);
            _cmd.Parameters.Add(tp);
            var lc = new List<string>(_paramNames);
            lc.Add(SkipParameterName);
            lc.Add(TakeParameterName);
            _paramNames = lc.ToArray();
        }

        public ResultSet<T> ExecutePagedQuery<T>(Func<IDataReader, T> mapper = null)
        {
            string select;
            string count;
            _db.Provider.MakePaged(_cmd.CommandText, out @select, out count);

            var rez = new ResultSet<T>();

            using (_cmd)
            {
                _cmd.CommandText = count;
                FormatCommand();
                try
                {
                    rez.Count = _cmd.ExecuteScalar().ConvertTo<int>();
                    _db.OnCommand(_cmd);
                    if (rez.Count == 0)
                    {
                        return rez;
                    }
                    var it = new List<T>();
                    _cmd.CommandText = select;
                    AddPagingParams();
                    FormatCommand();
                    using (var rd = _cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            if (mapper == null) mapper = PocoFactory.GetPocoMapper<T>(rd, _cmd.CommandText);
                            it.Add(mapper(rd));
                        }
                    }
                    rez.Items = it.ToArray();
                    _db.OnCommand(_cmd);
                }
                catch (Exception ex)
                {
                    Db.OnException(this, ex);
                    Db.CloseConnection();
                    throw;
                }
            }
            return rez;
        }

        public new IPagedQueryStatement ApplyToCommand(Action<DbCommand> action)
        {
            base.ApplyToCommand(action);
            return this;
        }
    }
}