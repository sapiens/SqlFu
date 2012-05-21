using System;
using System.Data.SqlClient;
using SqlFu;
using Tests;

namespace Benchmark
{
    public class Config
    {
        public const string Connex = "Data Source=.;Initial Catalog=tempdb;Integrated Security=True";
        public const string ConnectionName = "Sql";
        public const int Iterations = 500;
        public const int WarmUp = 10;
        public const string SqlServerProviderName = "System.Data.SqlClient";
        static Config()
        {
            EnsureDb();
        }

        public static SqlConnection GetOpenConnection()
        {
            var connection = new SqlConnection(Connex);
            connection.Open();
            return connection;
        }


        //public static void Benchmark(Stopwatch timer,Action bench,string name,bool warm=false)
        //{
        //    var nr = warm ? WarmUp : Iterations;
        //    timer.Restart();
        //    for (int i = 0; i < nr; i++) bench();
        //    timer.Stop();
        //    if (!warm) Console.WriteLine("{0} took {1} ms",name,timer.Elapsed.TotalMilliseconds);
        //}

        public static DbAccess GetDb()
        {
            var d = new DbAccess(Connex, DBType.SqlServer);
            return d;
        }

        private const string IfExistsDbSql =
//                        @"select case when EXISTS (SELECT * 
//                             FROM INFORMATION_SCHEMA.TABLES 
//                             WHERE TABLE_SCHEMA = 'dbo' 
//                             AND  TABLE_NAME = 'Posts') then 1 else 0 end;";

           @"select case when OBJECT_ID('sfPosts') is null then 0 else 1 end";
        const string CreateTableSql= @"
CREATE TABLE [sfPosts] (
[Id] int NOT NULL IDENTITY(1,1) ,
[Title] nvarchar(20) COLLATE Latin1_General_CI_AI NOT NULL ,
[AuthorId] int NOT NULL ,
[CreatedOn] datetime NOT NULL ,
[TopicId] int NULL ,
[IsActive] bit NOT NULL
CONSTRAINT [PK__sfPosts__3214EC070AD2A005] PRIMARY KEY ([Id])
)
ON [PRIMARY]
";
        public static void EmptyTable()
        {
            using (var db= GetDb())
            {
                db.ExecuteCommand("truncate table sfPosts;DBCC CHECKIDENT ('dbo.sfPosts', reseed, 1)");
            }
        }

        public static void EnsureDb()
        {
            var db = GetDb();
            
            if (!db.ExecuteScalar<bool>(IfExistsDbSql))
            {
                db.ExecuteCommand(CreateTableSql);
            };
         
        }

        public static void EnsurePosts()
        {
            var db = GetDb();
            db.KeepAlive = true;
            db.OnCommand = c => { };
            
            if (db.ExecuteScalar<int>("select count(*) from sfposts") != 15)
            {
                Config.EmptyTable();
             //   Console.WriteLine("ensuring 500 posts");

                using (var t = db.BeginTransaction())
                {
                    for (int i = 0; i < 15; i++)
                    {
                        db.Insert(new sfPosts {Title = "test" + i, AuthorId = 1, CreatedOn = DateTime.Now,IsActive = true});
                    }
                    t.Commit();
                }
            }
            db.Dispose();
        }
    }
}