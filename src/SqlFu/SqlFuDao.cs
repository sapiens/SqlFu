using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using CavemanTools;
using CavemanTools.Model;
using SqlFu.DDL;
using SqlFu.Internals;

namespace SqlFu
{
    public static class SqlFuDao
    {
        public const char EscapeSqlMarker = '$';

        public static int ExecuteCommand(this DbConnection cnx, string sql, params object[] args)
        {
            return cnx.Execute(sql, args);
        }

        public static int Execute(this DbConnection cnx, string sql, params object[] args)
        {
            using (var cmd = cnx.CreateAndSetupCommand(sql, args))
            {
                return cmd.Execute();
            }
        }

        /// <summary>
        /// Gets the table name (if set with [Table]) or the type name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnx"></param>
        /// <param name="escape"></param>
        /// <returns></returns>
        public static string GetTableName<T>(this DbConnection cnx, bool escape = true)
        {
            var name = TableInfo.ForType(typeof (T)).Name;
            if (escape)
            {
                return cnx.EscapeIdentifier(name);
            }
            return name;
        }


        public static T GetValue<T>(this DbConnection cnx, string sql, params object[] args)
        {
            using (var cmd = cnx.CreateAndSetupCommand(sql, args))
            {
                return cmd.GetValue(PocoFactory.GetConverter<T>());
            }
        }

        public static string EscapeSql(this IHaveDbProvider provider, string sql)
        {
            if (sql.IsNullOrEmpty()) return sql;
            if (sql.IndexOf(EscapeSqlMarker) < 0) return sql;
            var sb = new StringBuilder(sql.Length*2);
            bool inIdentifier = false;
            char[] delimiters = new[]
            {',', ' ', '\t', '\n', '\r', '=', '<', '>', ':', '(', ')', ']', '+', '-', '*', '/', '%'};
            List<char> identifier = new List<char>(16);
            foreach (var c in sql)
            {
                if (inIdentifier)
                {
                    if (c == EscapeSqlMarker || delimiters.Any(d => d == c))
                    {
                        inIdentifier = false;
                        if (c != EscapeSqlMarker)
                        {
                            sb.Append(provider.EscapeName(new string(identifier.ToArray())));
                        }
                        sb.Append(c);
                        identifier.Clear();
                    }
                    else
                    {
                        identifier.Add(c);
                    }
                    continue;
                }
                if (c == EscapeSqlMarker)
                {
                    inIdentifier = true;
                }
                else
                {
                    sb.Append(c);
                }
            }
            if (identifier.Count > 0)
            {
                sb.Append(provider.EscapeName(new string(identifier.ToArray())));
            }
            return sb.ToString();
        }

        public static string EscapeIdentifier(this DbConnection cnx, string identifier)
        {
            return cnx.GetProvider().EscapeName(identifier);
        }

        /// <summary>
        /// Escapes marked columns and table names from query. Identifier must be prefixed with '$'.
        /// Example: select $column, $other as column1 from dbo.$table -> select [column], [other] as column1 from dbo.[table]
        /// </summary>
        /// <param name="cnx"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string EscapeSql(this DbConnection cnx, string sql)
        {
            return cnx.GetProvider().EscapeSql(sql);
        }


        public static IControlSqlStatement WithSql(this DbConnection cnx, string sql, params object[] args)
        {
            return new ControlledQueryStatement(cnx, sql, args);
        }

        /// <summary>
        /// Returns only the first row of result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnx"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T QuerySingle<T>(this DbConnection cnx, string sql, params object[] args)
        {
            using (var cmd = cnx.CreateAndSetupCommand(sql, args))
            {
                return cmd.Fetch<T>(firstRowOnly: true).FirstOrDefault();
            }
        }

        /// <summary>
        /// Does a simple 'select * from T'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnx"></param>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(this DbConnection cnx)
        {
            var tableData = TableInfo.ForType(typeof (T));
            var sql = "select * from {0}".ToFormat(cnx.GetProvider().EscapeName(tableData.Name));
            return Query<T>(cnx, sql);
        }

        public static IEnumerable<T> Query<T>(this DbConnection cnx, string sql, params object[] args)
        {
            return Fetch<T>(cnx, sql, args);
        }

        public static List<T> Fetch<T>(this DbConnection cnx, string sql, params object[] args)
        {
            using (var cmd = cnx.CreateAndSetupCommand(sql, args))
            {
                return cmd.Fetch<T>();
            }
        }

        public static DbCommand CreateAndSetupCommand(this DbConnection cnx, string sql, params object[] args)
        {
            var provider = cnx.GetProvider();
            var cmd = cnx.CreateCommand();
            var statement = new PrepareStatement(provider, sql, args);
            statement.Setup(cmd);
            return cmd;
        }

        #region Multiple Resultsets

        public static Tuple<IList<T1>, IList<T2>> Fetch<T1, T2>(this DbConnection cnx, string sql, params object[] args)
            where T1 : new() where T2 : new()
        {
            using (var cmd = cnx.CreateAndSetupCommand(sql, args))
            {
                using (var reader = cmd.ExecuteLoggedReader())
                {
                    var list1 = MapReaderToModel<T1>(reader, cmd);
                    var list2 = MapReaderToModel<T2>(reader, cmd, true);

                    return new Tuple<IList<T1>, IList<T2>>(list1, list2);
                }
            }
        }

        /// <summary>
        /// Calls OnCommand and OnError 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static DbDataReader ExecuteLoggedReader(this DbCommand cmd)
        {
            try
            {
                var reader = cmd.ExecuteReader();
                OnCommand(cmd);
                return reader;
            }
            catch (DbException ex)
            {
                OnException(cmd, ex);
                throw;
            }
        }


        public static Tuple<IList<T1>, IList<T2>, IList<T3>> Fetch<T1, T2, T3>(this DbConnection cnx, string sql,
            params object[] args)
            where T1 : new() where T2 : new() where T3 : new()
        {
            using (var cmd = cnx.CreateAndSetupCommand(sql, args))
            {
                using (var reader = cmd.ExecuteLoggedReader())
                {
                    var list1 = MapReaderToModel<T1>(reader, cmd);
                    var list2 = MapReaderToModel<T2>(reader, cmd, true);
                    var list3 = MapReaderToModel<T3>(reader, cmd, true);

                    return new Tuple<IList<T1>, IList<T2>, IList<T3>>(list1, list2, list3);
                }
            }
        }

        public static Tuple<IList<T1>, IList<T2>, IList<T3>, IList<T4>> Fetch<T1, T2, T3, T4>(this DbConnection cnx,
            string sql, params object[] args)
            where T1 : new() where T2 : new() where T3 : new() where T4 : new()
        {
            using (var cmd = cnx.CreateAndSetupCommand(sql, args))
            {
                using (var reader = cmd.ExecuteLoggedReader())
                {
                    var list1 = MapReaderToModel<T1>(reader, cmd);
                    var list2 = MapReaderToModel<T2>(reader, cmd, true);
                    var list3 = MapReaderToModel<T3>(reader, cmd, true);
                    var list4 = MapReaderToModel<T4>(reader, cmd, true);

                    return new Tuple<IList<T1>, IList<T2>, IList<T3>, IList<T4>>(list1, list2, list3, list4);
                }
            }
        }

        public static Tuple<IList<T1>, IList<T2>, IList<T3>, IList<T4>, IList<T5>> Fetch<T1, T2, T3, T4, T5>(
            this DbConnection cnx, string sql, params object[] args)
            where T1 : new() where T2 : new() where T3 : new() where T4 : new() where T5 : new()
        {
            using (var cmd = cnx.CreateAndSetupCommand(sql, args))
            {
                using (var reader = cmd.ExecuteLoggedReader())
                {
                    var list1 = MapReaderToModel<T1>(reader, cmd);
                    var list2 = MapReaderToModel<T2>(reader, cmd, true);
                    var list3 = MapReaderToModel<T3>(reader, cmd, true);
                    var list4 = MapReaderToModel<T4>(reader, cmd, true);
                    var list5 = MapReaderToModel<T5>(reader, cmd, true);

                    return new Tuple<IList<T1>, IList<T2>, IList<T3>, IList<T4>, IList<T5>>(list1, list2, list3, list4,
                        list5);
                }
            }
        }

        public static Tuple<IList<T1>, IList<T2>, IList<T3>, IList<T4>, IList<T5>, IList<T6>> Fetch
            <T1, T2, T3, T4, T5, T6>(this DbConnection cnx, string sql, params object[] args)
            where T1 : new() where T2 : new() where T3 : new() where T4 : new() where T5 : new() where T6 : new()
        {
            using (var cmd = cnx.CreateAndSetupCommand(sql, args))
            {
                using (var reader = cmd.ExecuteLoggedReader())
                {
                    var list1 = MapReaderToModel<T1>(reader, cmd);
                    var list2 = MapReaderToModel<T2>(reader, cmd, true);
                    var list3 = MapReaderToModel<T3>(reader, cmd, true);
                    var list4 = MapReaderToModel<T4>(reader, cmd, true);
                    var list5 = MapReaderToModel<T5>(reader, cmd, true);
                    var list6 = MapReaderToModel<T6>(reader, cmd, true);

                    return new Tuple<IList<T1>, IList<T2>, IList<T3>, IList<T4>, IList<T5>, IList<T6>>(list1, list2,
                        list3, list4, list5, list6);
                }
            }
        }


        internal static IList<TModel> MapReaderToModel<TModel>(IDataReader reader, IDbCommand command,
            bool useNextResult = false)
        {
            var results = new List<TModel>();

            if (useNextResult)
            {
                if (!reader.NextResult())
                {
                    return results;
                }
            }

            while (reader.Read())
            {
                var mapper = PocoFactory.GetPocoMapper<TModel>(reader, command.CommandText);
                results.Add(mapper(reader));
            }

            return results;
        }

        #endregion

        #region Settings

        public static void ConnectionNameIs(string name)
        {
            name.MustNotBeEmpty();
            conexName = name;
        }

        public static void ConnectionStringIs(string conn, DbEngine engineType)
        {
            conn.MustNotBeEmpty();
            connString = conn;
            Engine = engineType;
        }

        public static DbConnection GetConnection()
        {
            if (!conexName.IsNullOrEmpty())
            {
                return new SqlFuConnection(conexName);
            }

            if (!connString.IsNullOrEmpty())
            {
                return new SqlFuConnection(connString, Engine);
            }
            return new SqlFuConnection();
        }

        /// <summary>
        /// Escapes any marked (prefixed with $) identifier according to executing db engine.
        /// This feature is here to help you with cross db compatibility.
        /// Default is false
        /// </summary>
        public static bool EscapeMarkedIdentifiers { get; set; }

        #endregion

        #region Events

        private static Action<DbCommand, Exception> _onException = (s, e) => { };

        public static Action<DbCommand, Exception> OnException
        {
            get { return _onException; }
            set
            {
                value.MustNotBeNull();
                _onException = value;
            }
        }


        private static Action<DbCommand> _onCmd = c => { };

        public static Action<DbCommand> OnCommand
        {
            get { return _onCmd; }
            set
            {
                value.MustNotBeNull();
                _onCmd = value;
            }
        }


        private static Action<DbConnection> _onCloseConex = c => { };

        public static Action<DbConnection> OnCloseConnection
        {
            get { return _onCloseConex; }
            set
            {
                value.MustNotBeNull();
                _onCloseConex = value;
            }
        }

        private static Action<DbConnection> _onOpenConex = c => { };
        private static string conexName;
        private static string connString;
        private static DbEngine Engine = DbEngine.SqlServer;

        public static Action<DbConnection> OnOpenConnection
        {
            get { return _onOpenConex; }
            set
            {
                value.MustNotBeNull();
                _onOpenConex = value;
            }
        }


        //private static Action<DbTransaction,int> _onBeginTransaction = (d,l) => { };

        //public static Action<DbTransaction,int> OnBeginTransaction
        //{
        //    get { return _onBeginTransaction; }
        //    set
        //    {
        //        value.MustNotBeNull();
        //        _onBeginTransaction = value;
        //    }
        //}

        //private static Action<bool> _onEndTransaction = (s) => { };

        ///// <summary>
        ///// Parameter is true if commit, false if rollback
        ///// </summary>
        //public static Action< bool> OnEndTransaction
        //{
        //    get { return _onEndTransaction; }
        //    set
        //    {
        //        value.MustNotBeNull();
        //        _onEndTransaction = value;
        //    }
        //} 

        #endregion

        public static IDatabaseTools DatabaseTools(this DbConnection cnx)
        {
            var sqlfu = cnx as SqlFuConnection;
            if (sqlfu == null)
            {
                throw new InvalidOperationException("Database tools are available only on SqlFu connection");
            }
            return sqlfu.DatabaseTools;
        }

        public static void Reset(this DbCommand cmd)
        {
            cmd.CommandText = "";
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.Text;
        }

        #region SProc

        /// <summary>
        /// Executes sproc
        /// </summary>
        /// <param name="cnx"></param>
        /// <param name="sprocName"></param>
        /// <param name="arguments">Arguments as an anonymous object, output parameters names must be prefixed with _ </param>
        /// <example>
        /// ExecuteStoredProcedure("sprocName",new{Id=1,_OutValue=""})
        /// </example>
        /// <returns></returns>
        public static StoredProcedureResult ExecuteStoredProcedure(this DbConnection cnx, string sprocName,
            object arguments)
        {
            using (var cmd = cnx.CreateCommand())
            {
                var output = new List<IDbDataParameter>();
                sprocName.MustNotBeEmpty();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = sprocName;
                var provider = cnx.GetProvider();
                var paramDict = PrepareStatement.CreateParamsDictionary(arguments);
                var allp = cmd.Parameters;

                IDbDataParameter p = null;

                p = cmd.CreateParameter();
                provider.SetupParameter(p, "ReturnValue", 0);
                allp.Add(p);
                var returnParam = p;
                p.Direction = ParameterDirection.ReturnValue;

                foreach (var kv in paramDict)
                {
                    p = cmd.CreateParameter();
                    var pname = kv.Key;
                    if (kv.Key.StartsWith("_"))
                    {
                        pname = kv.Key.Substring(1);
                        p.Direction = ParameterDirection.Output;
                    }
                    provider.SetupParameter(p, pname, kv.Value);
                    allp.Add(p);
                    output.Add(p);
                }

                cmd.Execute();
                var rez = new StoredProcedureResult();
                rez.ReturnValue = returnParam.Value.ConvertTo<int>();
                foreach (var param in output)
                {
                    //first char of parameter name is db specific param prefix
                    rez.OutputValues[param.ParameterName.Substring(1)] = param.Value;
                }

                return rez;
            }
        }

        #endregion

        public static PagedResult<T> PagedQuery<T>(this DbConnection cnx, Pagination pager, string sql,
            params object[] args)
        {
            return PagedQuery<T>(cnx, pager.Skip, pager.PageSize, sql,args);
        }

    public static PagedResult<T> PagedQuery<T>(this DbConnection cnx, long skip, int take, string sql,
                                                   params object[] args)
        {
            var rez = new PagedResult<T>();
            using (var cmd = cnx.CreateCommand())
            {
                var statement = new PreparePagedStatement(cnx.GetProvider(), skip, take, sql, args);
                statement.SetupCount(cmd);
                try
                {
                    var cnt = cmd.ExecuteScalar();
                    cnt.MustNotBeNull();
                    if (cnt.GetType() == typeof (Int32))
                    {
                        rez.Count = (int) cnt;
                    }
                    else
                    {
                        rez.LongCount = (long) cnt;
                    }
                    OnCommand(cmd);
                }
                catch (Exception ex)
                {
                    OnException(cmd, ex);
                    throw;
                }

                if (rez.LongCount >0)
                {
                    statement.Setup(cmd);
                    rez.Items = cmd.Fetch<T>();
                }

                return rez;
            }
        }

        #region Command actions

        /// <summary>
        /// Identically with ExecuteScalar but has logging enabled
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static object GetRawValue(this DbCommand cmd)
        {
            object rez;
            try
            {
                rez = cmd.ExecuteScalar();
                OnCommand(cmd);
                return rez;
            }
            catch (Exception ex)
            {
                OnException(cmd, ex);
                throw;
            }
        }

        public static T GetValue<T>(this DbCommand cmd, Func<object, T> converter)
        {
            object rez;
            try
            {
                rez = cmd.ExecuteScalar();
                OnCommand(cmd);
                return converter(rez);
            }
            catch (Exception ex)
            {
                OnException(cmd, ex);
                throw;
            }
        }

        public static int Execute(this DbCommand cmd)
        {
            int rez;
            try
            {
                rez = cmd.ExecuteNonQuery();
                OnCommand(cmd);
                return rez;
            }
            catch (Exception ex)
            {
                OnException(cmd, ex);
                throw;
            }
        }

        public static List<T> Fetch<T>(this DbCommand cmd, Func<IDataReader, T> mapper = null,
                                       bool firstRowOnly = false)
        {
            List<T> rez = new List<T>();

            try
            {
                CommandBehavior behavior = firstRowOnly ? CommandBehavior.SingleRow : CommandBehavior.Default;
                using (var reader = cmd.ExecuteReader(behavior))
                {
                    OnCommand(cmd);
                    while (reader.Read())
                    {
                        if (mapper == null)
                        {
                            mapper = PocoFactory.GetPocoMapper<T>(reader, cmd.CommandText);
                        }
                        rez.Add(mapper(reader));
                    }
                }

                return rez;
            }
            catch (DbException ex)
            {
                OnException(cmd, ex);
                throw;
            }
        }

        #endregion

        #region OtherHelpers

        #endregion
    }
}