using System;
using System.Data.SqlClient;
using System.Diagnostics;
using SqlFu;

namespace Tests
{
    public class Config
    {
        public const string Connex = "Data Source=.;Initial Catalog=tempdb;Integrated Security=True";
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


        public static void Benchmark(Stopwatch timer,Action bench,string name,bool warm=false)
        {
            var nr = warm ? WarmUp : Iterations;
            timer.Restart();
            for (int i = 0; i < nr; i++) bench();
            timer.Stop();
            if (!warm) Console.WriteLine("{0} took {1} ms",name,timer.Elapsed.TotalMilliseconds);
        }

       public static DbAccess GetDb(bool noLog=false)
        {
            var d=new DbAccess(Connex, DBType.SqlServer);
           if (!noLog)
           {
               d.OnCommand = cmd => Console.WriteLine(cmd.FormatCommand());
               d.OnOpenConnection = cmd => Console.WriteLine("Cnnection opened");
               d.OnCloseConnection = cmd => Console.WriteLine("Cnnection closed");
               d.OnException = (s, ex) =>
               {
                   Console.WriteLine(s.ExecutedSql);
               };

               d.OnBeginTransaction = cmd => Console.WriteLine("Begin trans");
               d.OnEndTransaction = (cmd, s) => Console.WriteLine("End trans: {0}", s);    
           }
           
           return d;
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
            using (var db= GetDb())
            {
                db.ExecuteCommand("truncate table Posts;DBCC CHECKIDENT ('dbo.Posts', reseed, 1)");
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
            if (db.ExecuteScalar<int>("select count(*) from posts") != 10)
            {
                Config.EmptyTable();
                Console.WriteLine("ensuring 10 posts");

                using (var t = db.BeginTransaction())
                {
                    for (int i = 0; i < 10; i++)
                    {
                        db.Insert(new Post {Title = "test" + i, AuthorId = 1, CreatedOn = DateTime.Now});
                    }
                    t.Commit();
                }
            }
            db.Dispose();
        }
    }
}