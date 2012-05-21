//#define ONLY_TESTS

//using System.Data.Common;
//using FluentData;
//using PetaPoco;
//using ServiceStack.OrmLite;
//using ServiceStack.OrmLite.SqlServer;
//using SqlFu;
//using Xunit;
//using System;
//using System.Diagnostics;
//using System.Linq;
//using Dapper;
//using IDbCommand = System.Data.IDbCommand;

//namespace Tests
//{
//    public class Benchmark:IDisposable
//    {
//        private Stopwatch _t = new Stopwatch();
//        private DbAccess _db;

//        public Benchmark()
//        {
//            _db = Config.GetDb(true);
//            _db.OnCommand = c => { };
//            _db.KeepAlive = true;
//            Config.EnsurePosts();            
//        }
//#if !ONLY_TESTS
//        [Fact]
//        public void run_all_Paged()
//        {
//            peta_pagination();
//            sqlfu_pagination();            
//        }

//        [Fact]
//        public void run_all_complex()
//        {
//            fluent_data_complex();
//            sqlfu_query_complex();
//        }

//        [Fact]
//        public void run_all_query()
//        {
//            sqlfu_query();
//            dapper_query();
//            peta_query();
//            ormlite_query();
//            fluent_data_query();
//        }
//#endif
//        #region SqlFu
//      //  [Fact]
//        public void sqlfu_pagination()
//        {
//            Action qr = () =>
//                         {
//                             _db.PagedQuery<Post>(0, 5, "select * from posts where id>@0 order by id", 3);
//                         };
//            Config.Benchmark(_t, qr, "SqlFu paged query", true);
//            Config.Benchmark(_t, qr, "SqlFu paged query");
//        }

//        //[Fact]
//        public void sqlfu_query()
//        {
//            Action qr = () =>
//            {
//                _db.Query<Post>("select * from posts where id>@0 order by id", 3).ToArray();
//            };
//            Config.Benchmark(_t, qr, "SqlFu  query", true);
//            Config.Benchmark(_t, qr, "SqlFu  query");
//        }

//      //  [Fact]
//        public void sqlfu_query_complex()
//        {
//            Action qr = () =>
//            {
//                _db.Query<PostViewModel>("select p.id,p.title,p.Id as Author_Id from posts p where id>@0 order by id", 3).ToArray();
//            };
//            Config.Benchmark(_t, qr, "SqlFu complex query", true);
//            Config.Benchmark(_t, qr, "SqlFu complex query");
//        } 
//        #endregion

//        #region Peta poco
//      //  [Fact]
//        public void peta_pagination()
//        {
//            var p = new Database(Config.Connex, "System.Data.SqlClient");
//            p.EnableAutoSelect = false;
//            p.EnableNamedParams = false;
//            p.KeepConnectionAlive = true;
//            p.OpenSharedConnection();
//            Action qr = () =>
//            {
//                p.Page<Post>(0, 5, "select * from posts where id>@0 order by id", 3);
//            };
//            Config.Benchmark(_t, qr, "Peta poco paged query", true);
//            Config.Benchmark(_t, qr, "Peta poco paged query");
//        }

//      //  [Fact]
//        public void peta_query()
//        {
//            var p = new Database(Config.Connex, "System.Data.SqlClient");
//            p.EnableAutoSelect = false;
//            p.EnableNamedParams = false;
//            p.KeepConnectionAlive = true;
//            p.OpenSharedConnection();
//            Action qr = () =>
//            {
//                p.Query<Post>("select * from posts where id>@0 order by id", 3).ToArray();
//            };
//            Config.Benchmark(_t, qr, "Peta poco  query", true);
//            Config.Benchmark(_t, qr, "Peta poco  query");
//        } 
//        #endregion

//      //  [Fact]
//        public void dapper_query()
//        {
//            var cnx = DbProviderFactories.GetFactory(Config.SqlServerProviderName).CreateConnection();
//            cnx.ConnectionString = Config.Connex;
//            cnx.Open();
//            Action qr = () =>
//            {
//                cnx.Query<Post>("select Id,AuthorId,Title,CreatedOn,'Post' as Type,TopicId,IsActive from posts where id>@id order by id", new{id=3}).ToArray();
//            };
//            Config.Benchmark(_t, qr, "Dapper  query", true);
//            Config.Benchmark(_t, qr, "Dapper  query");
//            cnx.Close();
//        }

//        #region OrmLite
//     //   [Fact]
//        public void ormlite_query()
//        {
//            OrmLiteConfig.DialectProvider = SqlServerOrmLiteDialectProvider.Instance; //Using SQL Server
            
//            IDbCommand ormLiteCmd = Config.GetOpenConnection().CreateCommand();

//            Action qr = () =>
//            {
//                ormLiteCmd.Query<Post>("select * from posts where id>@id order by id",new{id=3});
//            };
//            Config.Benchmark(_t, qr, "Ormlite  query", true);
//            Config.Benchmark(_t, qr, "Ormlite  query");
//            ormLiteCmd.Connection.Dispose();
//            ormLiteCmd.Dispose();
//        }

//       #endregion

//        #region FluentData

//   //     [Fact]
//        public void fluent_data_query()
//        {
//            var fd = new DbContext().ConnectionString(Config.Connex, DbProviderTypes.SqlServer);
//            Action qr = () =>
//                            {
//                                fd.Sql("select * from posts where id>@0 order by id", 3).Query<Post>();
//                            };
//            Config.Benchmark(_t, qr, "Fluent data  query", true);
//            Config.Benchmark(_t, qr, "Fluent data  query");
//            fd.Dispose();
//        }

//       // [Fact]
//        public void fluent_data_complex()
//        {
//            var fd = new DbContext().ConnectionString(Config.Connex, DbProviderTypes.SqlServer);
//            Action qr = () =>
//            {
//                fd.Sql("select p.Id,p.title,p.Id as Author_Id from posts p where id>@0 order by id",3).Query<PostViewModel>();
//            };
//            Config.Benchmark(_t, qr, "Fluent data query complex", true);
//            Config.Benchmark(_t, qr, "Fluent data query complex");
//            fd.Dispose(); 
//        }

//        #endregion

//        private void Write(string format, params object[] param)
//        {
//            Console.WriteLine(format, param);
//        }

//        public void Dispose()
//        {
//            _db.Dispose();            
//        }
//    }
//}