using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Text;
using CavemanTools.Model;
using SqlFu.DDL;
using SqlFu.Internals;

namespace SqlFu
{
    public class DbAccess :IAccessDb
    {
        public DbAccess()
        {
            var cnx=ConfigurationManager.ConnectionStrings[0];
            if (cnx==null) throw new InvalidOperationException("I need a connection!!!");
            
            Init(cnx.ConnectionString,ProviderFactory.GetProviderByName(cnx.ProviderName));
        }

        public DbAccess(string connectionStringName)
        {
            var cnx = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (cnx == null) throw new InvalidOperationException("I need a connection!!!");

            Init(cnx.ConnectionString, ProviderFactory.GetProviderByName(cnx.ProviderName));
        }

        public DbAccess(string cnxString,string provider)
        {
            Init(cnxString,ProviderFactory.GetProviderByName(provider));
        }

        public DbAccess(string cnxString,DbEngine provider)
        {
          Init(cnxString,ProviderFactory.GetProvider(provider)); 
        }

        public DbAccess(string cnxString,IHaveDbProvider provider)
        {
            Init(cnxString, provider);
        }

        private void Init(string cnxString, IHaveDbProvider provider)
        {
            cnxString.MustNotBeEmpty();
            provider.MustNotBeNull();
            _provider = provider;
            _cnxString = cnxString;
            KeepAlive = false;
        }

        #region SProc

         /// <summary>
        /// Executes sproc
        /// </summary>
        /// <param name="sprocName"></param>
        /// <param name="arguments">Arguments as an anonymous object, output parameters names must be prefixed with _ </param>
        /// <example>
        /// ExecuteStoredProcedure("sprocName",new{Id=1,_OutValue=""})
        /// </example>
        /// <returns></returns>
        public StoredProcedureResult ExecuteStoredProcedure(string sprocName, object arguments=null)
        {
            var sql = new SProcStatement(this);
            sql.UseStoredProcedure(sprocName, arguments);
            return sql.Execute();
        }

        #endregion

        public IDatabaseTools DatabaseTools
        {
            get { return Provider.GetTools(this); }
        }

        private DbConnection _conex;
        
        public DbConnection Connection
        {
            get
            {
                if (_conex == null)
                {
                    _conex = Provider.CreateConnection();
                    _conex.ConnectionString = _cnxString;
                    _conex.Open();
                    OnOpenConnection(this);
                }
                return _conex;
            }
        }

        public bool KeepAlive { get; set; }

        public IHaveDbProvider Provider
        {
            get { return _provider; }
        }

        private IHaveDbProvider _provider;
        private Action<DbCommand> _onCmd = c => { };

        public Action<DbCommand> OnCommand
        {
            get { return _onCmd; }
            set
            {
                value.MustNotBeNull();
                _onCmd = value;
            }
        }

        private Action<IAccessDb> _onCloseConex = c => { };

        public Action<IAccessDb> OnCloseConnection
        {
            get { return _onCloseConex; }
            set
            {
                value.MustNotBeNull();
                _onCloseConex = value;
            }
        }

        private Action<IAccessDb> _onOpenConex = c => { };

        public Action<IAccessDb> OnOpenConnection
        {
            get { return _onOpenConex; }
            set
            {
                value.MustNotBeNull();
                _onOpenConex = value;
            }
        }

        private Action<ISqlStatement,Exception> _onException = (s,e) => { };

        public Action<ISqlStatement, Exception> OnException
        {
            get { return _onException; }
            set
            {
                value.MustNotBeNull();
                _onException = value;
            }
        }

        private Action<IAccessDb> _onBeginTransaction = (d) => { };

        public Action<IAccessDb> OnBeginTransaction
        {
            get { return _onBeginTransaction; }
            set
            {
                value.MustNotBeNull();
                _onBeginTransaction = value;
            }
        }

        private Action<IAccessDb,bool> _onEndTransaction = (d,s) => { };

        public Action<IAccessDb, bool> OnEndTransaction
        {
            get { return _onEndTransaction; }
            set
            {
                value.MustNotBeNull();
                _onEndTransaction = value;
            }
        }

        internal DbCommand CreateCommand()
        {
            var cmd = Connection.CreateCommand();
            if (_trans != null) cmd.Transaction = _trans;
            return cmd;
        }

        /// <summary>
        /// Prepares sql statement
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ISqlStatement WithSql(string sql, params object[] args)
        {
            var st = new SqlStatement(this);
            st.SetSql(sql, args);
            return st;
        }

        public IPagedSqlStatement WithSql(long skip,int take,string sql, params object[] args)
        {
            var st = new PagedSqlStatement(this);
            st.SetSql(skip,take,sql,args);
            return st;
        }

        public void CloseConnection(bool forceClose = false)
        {
            if (_conex != null)
            {
                if (!forceClose && (KeepAlive ||(_trans!=null))) return;

                _conex.Close();
                OnCloseConnection(this);
                _conex = null;
            }
        }


        public static DbType Convert(Type t)
        {
            //if (t==typeof(int))
            //{
            //    return DbType.Int32;
            //}
            
            //if (t==typeof(int))
            //{
            //    return DbType.;
            //}

            throw new NotSupportedException();
        }
        
        #region Transaction support
        private int _tLevel = 0;

        
        public int TransactionDepth
        {
            get { return _tLevel; }
        }

        private DbTransaction _trans = null;

        private string _cnxString;

        public DbTransaction BeginTransaction(IsolationLevel? isolationLevel = null)
        {
            _tLevel++;
            if (_tLevel == 1)
            {
                _trans = isolationLevel.HasValue ? Connection.BeginTransaction(isolationLevel.Value) : Connection.BeginTransaction();
            }
            OnBeginTransaction(this);
            return new MyTransactionWrapper(this);
        }

        private void Commit()
        {
            EnsureTransaction();
            _tLevel--;
            if (_tLevel == 0)
            {
                _trans.Commit();
                _trans = null;               
            }
            OnEndTransaction(this, true);
        }

        private void Rollback()
        {
            if (_trans != null)
            {
                _trans.Dispose();
                _trans = null;
                _tLevel = 0;
                OnEndTransaction(this,false);
            }

        }

        private void EnsureTransaction()
        {
            if (_trans == null) throw new InvalidOperationException("No transaction started");
        }

        
        #region TrnsactionClass
        class MyTransactionWrapper : DbTransaction
        {
            private DbAccess _db;

            public MyTransactionWrapper(DbAccess db)
            {
                _db = db;
            }

            protected override void Dispose(bool disposing)
            {
                if (_db != null)
                {
                    Rollback();
                }

            }

            public override void Commit()
            {
                if (_db != null)
                {
                    _db.Commit();
                    _db = null;

                }
                else
                {
                    throw new InvalidOperationException("Transaction was finished");
                }
            }

            public override void Rollback()
            {
                _db.Rollback();
                _db = null;
            }

            protected override DbConnection DbConnection
            {
                get { return _db.Connection; }
            }

            public override  IsolationLevel IsolationLevel
            {
                get { return _db._trans.IsolationLevel; }
            }
        }
        #endregion

        #endregion

      
        #region Query

        public int ExecuteCommand(string sql, params object[] args)
        {
            using (var st=new SqlStatement(this))
            {
                st.SetSql(sql,args);
                return st.Execute();
            }
        }

        [Obsolete("Use GetValue method")]
        public T ExecuteScalar<T>(string sql, params object[] args)
        {
            return GetValue<T>(sql, args);
        }

        public T GetValue<T>(string sql, params object[] args)
        {
            using(var st= new SqlStatement(this))
            {
                st.SetSql(sql,args);
                return st.ExecuteScalar<T>();
            }
        }

        

        public T Get<T>(object id,string additionalPredicate=null,params object[] args)
        {
            id.MustNotBeNull("id");
            var tp = typeof (T);
            if (typeof(ExpandoObject) == tp || typeof(object) == tp)
                throw new InvalidOperationException("Can't work with System.Object or dynamic types");
            var ti = TableInfo.ForType(tp);
            if (ti.SelectSingleSql==null)
            {
                var sb = new StringBuilder("select ");
                var p = tp.GetProperties().Where(pr=>!pr.PropertyType.IsCustomObjectType()).Select(pr => pr.Name);
                foreach (var name in p)
                {
                    sb.AppendFormat("{0},", Provider.EscapeName(name));
                }
                sb.Remove(sb.Length - 1, 1);

                sb.AppendFormat(" from {0}", Provider.EscapeName(ti.Name));
                sb.AppendFormat(" where {0}=@0", Provider.EscapeName(ti.PrimaryKey));
                if (!string.IsNullOrEmpty(additionalPredicate))
                {
                    sb.AppendFormat(" {0}", additionalPredicate);
                }
                ti.SelectSingleSql = sb.ToString();
            }
            var rez = default(T);
            var fargs = new List<object>(args.Length + 1);
            fargs.Add(id);
            fargs.AddRange(args);
            using(var st=new SqlStatement(this))
            {
                st.SetSql(ti.SelectSingleSql,fargs.ToArray());
                
                using(var rd=st.GetReader())
                {
                    try
                    {
                        if (rd.Read())
                    {
                        Func<IDataReader, T> mapper = PocoFactory.GetPocoMapper<T>(rd,st.Sql);
                        rez = mapper(rd);
                    }}
                    catch(Exception ex)
                    {
                        OnException(st,ex);
                        throw;
                    }
                    finally
                    {
                        CloseConnection();
                    }
                }
            }
            return rez;
        }

        public ResultSet<T> PagedQuery<T>(long skip,int take,string sql, params object[] args)
        {
            using (var st= new PagedSqlStatement(this))
            {
                st.SetSql(skip,take,sql,args);
                return st.ExecutePagedQuery<T>();
            }
        }

        public IEnumerable<T> Query<T>(string sql, params object[] args)
        {
           
            using (var st = new SqlStatement(this))
           {
               
                st.SetSql(sql, args);
               foreach(var r in st.ExecuteQuery<T>())
               {
                   yield return r;
               }
               //using (var rd = st.GetReader())
               //{

               //    Func<IDataReader, T> mapper = null;
               //    T rez=default(T);
               //    while (true)
               //    {
               //        try
               //        {
               //            if (rd.Read())
               //            {

               //                if (mapper == null)
               //                {
               //                    mapper = PocoFactory.GetPocoMapper<T>(rd);
               //                }


               //                rez = mapper(rd);


               //            }
               //            else
               //            {
               //              break;
               //            }

               //        }
               //        catch (Exception ex)
               //        {
               //            OnException(st,ex);
               //            throw;
               //        }
               //        yield return rez;
               //    }
               //    //yield break;
               //} 

           }
            

        }

        public List<T> Fetch<T>(string sql, params object[] args)
        {
            using (var st = new SqlStatement(this))
            {

                st.SetSql(sql, args);

                using (var rd = st.GetReader())
                {

                    Func<IDataReader, T> mapper = null;
                    var rez = new List<T>();
                    try
                    {
                        while(rd.Read())
                        {
                            if (mapper == null)
                            {
                                mapper = PocoFactory.GetPocoMapper<T>(rd,st.Sql);
                            }
                            rez.Add(mapper(rd));
                        }
                    }
                    catch (Exception ex)
                    {
                        OnException(st,ex);
                        
                        throw;
                    }

                    return rez;
                }
            }
        }


        #endregion

        public void Dispose()
        {
            if (_trans != null)
            {
                Rollback();
            }
            CloseConnection(true);
        }

      
    }

    public class LastInsertId
    {
        private object _val;
        public static LastInsertId Empty = new LastInsertId(null);
        public LastInsertId(object o)
        {
            _val = o;
        }

        public bool IsEmpty
        {
            get { return _val == null; }
        }

        public T InsertedId<T>()
        {
            return _val.ConvertTo<T>();
        }
    }
}