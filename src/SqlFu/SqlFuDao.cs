using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using CavemanTools.Model;
using SqlFu.DDL;

namespace SqlFu
{
    public static class SqlFuDao
    {
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

        public static T GetValue<T>(this DbConnection cnx, string sql, params object[] args)
        {
            using (var cmd = cnx.CreateAndSetupCommand(sql, args))
            {
                return cmd.GetValue(PocoFactory.GetConverter<T>());
            }
        }

        public static string EscapeIdentifier(this DbConnection cnx, string identifier)
        {
            return cnx.GetProvider().EscapeName(identifier);
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

                if (rez.Count > 0)
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