using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using SqlFu;

namespace Tests
{
    
    public static class Setup
    {
        public const string Connex = "Data Source=.;Initial Catalog=tempdb;Integrated Security=True";
        public const string SqliteConnex = @"data source={0};foreign keys=True";
        public const string MysqlConnex = @"Server=localhost;Database=mysql;Uid=root;Pwd=;Allow User Variables=True";
        public const string PostgresConnex = @"User ID=postgres;Password=123456;Host=localhost;Port=5432;Database=postgres;";

        static Setup()
        {
            SqlFuDao.OnCommand = cmd => Console.WriteLine(cmd.FormatCommand());
            SqlFuDao.OnException = (cmd,ex)=>Console.WriteLine("\nSql:{1}\nException:\n\t\t{0}", ex,cmd.FormatCommand());
        }

        public static SqlFuConnection GetDb(bool noLog = false,DbEngine engine=DbEngine.SqlServer)
        {
            var cnx = Connex;
            switch(engine)
            {
                case DbEngine.SQLite:
                    DirectoryInfo dd = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                    var f = dd.Parent.Parent.FullName;
                    cnx = string.Format(SqliteConnex, Path.Combine(f, "testdb.db"));                    
                    //cnx = SqliteConnex;
                    break;
                case DbEngine.MySql:
                    cnx = MysqlConnex;
                    break;
                case DbEngine.PostgreSQL:
                    cnx = PostgresConnex;
                    break;
            }
            var d = new SqlFuConnection(cnx, engine);
            
            return d;
        }
    }

    public class Config
    {
        
        public const int Iterations = 500;
        public const int WarmUp = 10;
        public const string SqlServerProviderName = "System.Data.SqlClient";
        
        static Config()
        {
            EnsureDb();
        }

        public static SqlFuConnection GetDb()
        {
            return Setup.GetDb();
        }

        public static SqlConnection GetOpenConnection()
        {
            var connection = new SqlConnection(Setup.Connex);
            connection.Open();
            return connection;
        }


        public static void Benchmark(Stopwatch timer,Action bench,string name,bool warm=false)
        {
            var nr = warm ? WarmUp : Iterations;
            timer.Restart();
            for (int i = 0; i < nr; i++) bench();
            timer.Stop();
            if (!warm) Console.WriteLine("{0} took {1} ms",name,timer.Elapsed.TotalMilliseconds);
        }

      

        private const string IfExistsDbSql =
//                        @"select case when EXISTS (SELECT * 
//                             FROM INFORMATION_SCHEMA.TABLES 
//                             WHERE TABLE_SCHEMA = 'dbo' 
//                             AND  TABLE_NAME = 'Posts') then 1 else 0 end;";

           @"select case when OBJECT_ID('Posts') is null then 0 else 1 end";
        const string CreateTableSql= @"
CREATE TABLE [Posts] (
[Id] int NOT NULL IDENTITY(1,1) ,
[Title] nvarchar(20) COLLATE Latin1_General_CI_AI NOT NULL ,
[AuthorId] int NOT NULL ,
[CreatedOn] datetime NOT NULL ,
[Type] tinyint NOT NULL ,
[TopicId] int NULL ,
[IsActive] bit NOT NULL
CONSTRAINT [PK__Posts__3214EC070AD2A005] PRIMARY KEY ([Id])
)
ON [PRIMARY]
";

        public static void EmptyTable()
        {
            using (var db = Setup.GetDb())
            {
                db.ExecuteCommand("truncate table Posts;DBCC CHECKIDENT ('dbo.Posts', reseed, 1)");
            }
        }

        public static void EnsureDb()
        {
            var db = Setup.GetDb();
            
            if (!db.GetValue<bool>(IfExistsDbSql))
            {
                db.ExecuteCommand(CreateTableSql);
            };
         
        }

        public static void EnsurePosts()
        {
            var db = Setup.GetDb();
            
            EnsureDb();
                //Config.EmptyTable();
                //Console.WriteLine("ensuring 10 posts");

                using (var t = db.BeginSqlFuTransaction())
                {
                    for (int i = 0; i < 10; i++)
                    {
                        db.Insert(new Post {Title = "test" + i, AuthorId = 1, CreatedOn = DateTime.Now});
                    }
                    t.Commit();
                }
           
            db.Dispose();
        }
    }
}