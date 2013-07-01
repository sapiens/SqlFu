using System;
using System.Data.Common;
using SqlFu;
using SqlFu.DDL;

namespace Tests.SqlFu45
{
    [Table("Posts",CreationOptions = IfTableExists.DropIt)]
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string  Content { get; set; }
        public int AuthorId { get; set; }
    }
    
    public class Config
    {
        public const string Connex = "Data Source=.;Initial Catalog=tempdb;Integrated Security=True";
        static Config()
        {
            SqlFuDao.OnException = (cmd, ex) => Console.WriteLine("\nSql:{1}\nException:\n\t\t{0}", ex, cmd.FormatCommand()); 
            SqlFuDao.OnCommand = cmd => Console.WriteLine(cmd.FormatCommand());
            SqlFuDao.ConnectionStringIs(Connex,DbEngine.SqlServer);
        }
        public static DbConnection GetDb()
        {
            return SqlFuDao.GetConnection();
        }

        public static void CleanUp()
        {
            using (var db = GetDb())
            {
                db.Drop<Post>();
            }
        }

        public static void EnsurePosts()
        {
            using (var db = GetDb())
            {
                db.CreateTable<Post>();
                var random = new Random();
                using (var t = db.BeginTransaction())
                {
                    for (int i = 0; i < 20; i++)
                    {
                        var p = new Post()
                            {
                                AuthorId = random.Next(),
                                Content = "".GenerateRandomString(50),
                                Title = "".GenerateRandomString(10)
                            };
                        db.Insert(p);
                    }
                    t.Commit();
                }
                
            }
        }
    }
}