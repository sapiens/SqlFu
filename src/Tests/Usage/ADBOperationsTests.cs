using System;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using SqlFu;
using SqlFu.Builders.Expressions;
using SqlFu.Providers;
using Xunit;

namespace Tests.SqlServer
{
    public enum TSqlDatePart
    {
        Year,
        Day,
        Hour
        //etc
    }
    public static class MyExtensions
    {
        public static int DateDiff<T>(this T table, TSqlDatePart part, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }

    public class MyFunctions : DbProviderExpressions
    {
        public MyFunctions()
        {
            LinkMethods(() => 1.DateDiff(TSqlDatePart.Day, DateTime.Now, DateTime.Now), DateDiff);
        }

        private void DateDiff(MethodCallExpression method, StringBuilder sb, IGenerateSqlFromExpressions writer)
        {
            sb.Append("datediff(");
            sb.Append(method.Arguments[1].GetValue().ToString()).Append(",");
            sb.Append(writer.GetColumnsSql(method.Arguments[2])).Append(",");
            sb.Append(writer.GetColumnsSql(method.Arguments[3]));
            sb.Append(")");
        }

    }
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
      
        public bool IsDeleted { get; set; }
        public int Posts { get; set; }
        public DateTime CreatedOn { get; set; }=DateTime.Now;
        public string Category { get; set; } = Type.Post.ToString();
        public User()
        {
         
        }
    }

    public enum Type
    {
        Post,
        Page
    }

    public class ADBOperationsTests:IDisposable
    {
        protected DbConnection _db;


        public ADBOperationsTests(DbConnection db)
        {
            _db = db;
            _db.Provider().ReplaceExpressionsProvider(new MyFunctions());
            _db.CreateTableFrom<User>(cf =>
            {
                cf.HandleExisting(TableExistsAction.DropIt)
                    .TableName("users")
                    .Column(t => t.Id, c => c.AutoIncrement())
                    .ColumnSize(c=>c.FirstName,150)
                    .ColumnSize(c=>c.LastName,150)
                    .Column(d=>d.Category,c=>c
                                            .HasDbType(SqlServerType.Varchar)
                                            .HasSize(10)
                                            .HasDefaultValue(Type.Page.ToString()))
                 
                    .PrimaryKey(t=>t.OnColumns(d=>d.Id));
            });
            _db.Insert(new User()
            {
                FirstName = "John",
                LastName = "Doe"
            });

            _db.Insert(new User()
            {
                FirstName = "John1",
                LastName = "Doe",
                Posts = 3
            });

              _db.Insert(new User()
            {
                FirstName = "Jane",
                LastName = "Doe",
                Category = Type.Page.ToString(),
                Posts = 0
            });

           
        }


        [Fact]
        public void insert_and_return_id()
        {

            _db.Insert(new User()
            {
                FirstName = "Jane2",
                LastName = "Doe",
                Category = Type.Page.ToString(),
                Posts = 0
            }).GetInsertedId<int>().Should().Be(4);
        }


        [Fact]
        public void get_user_with_fullname()
        {
            var user=_db.QueryRow(q => q.From<User>()
                .Where(d => !d.IsDeleted && d.Category==Type.Page.ToString())
                .And(d => d.InjectSql("Posts= @no", new {no = 0}))
                .Select(d=>new {d.Id,Fullname=d.Concat(d.FirstName," ",d.LastName)}));
            user.Fullname.Should().Be("Jane Doe");
            user.Id.Should().Be(3);
            
        }

        [Fact]
        public void count_users()
        {
            _db.QueryValue<int>(q => q.From<User>().Select(d => d.Count())).Should().Be(3);
        }

        [Fact]
        public void datediff()
        {
            var dif=_db.QueryValue(q => q.From<User>().Select(d => d.DateDiff(TSqlDatePart.Hour, d.CreatedOn, DateTime.Now.AddHours(3))));
            dif.Should().Be(3);
        }

        [Fact]
        public void sum_of_posts()
        {
            var all=_db.QueryAs(q => q.From<User>()
                .GroupBy(d => d.Category)
                .Select(d => new {d.Category, total= d.Sum(d.Posts + 1)})
                ).ToDictionary(d=>d.Category.ToEnum<Type>(),d=>d.total);
            all[Type.Post].Should().Be(5);
            all[Type.Page].Should().Be(1);
        }


        [Fact]
        public void delete_where()
        {
            _db.DeleteFromAnonymous(new {Category = ""}, "users",
                d => d.Category == Type.Page.ToString())
                .Should().Be(1);
            _db.CountRows<User>().Should().Be(2);
        }

      
        [Fact]
        public async Task get_single_row_no_result()
        {
            var user=await _db.QueryRowAsync(q => q.From<User>().Where(d => d.Id == 1000).Limit(1).SelectAll(useAsterisk: true),
                CancellationToken.None);
            user.Should().BeNull();
        }

        [Fact]
        public void update_one_row()
        {
            _db.UpdateFrom(
                q => q.Data(new { Firstname = "John3", Id = 3 }).Ignore(d => d.Id)
                ,o => o.SetTableName("users")
                )
                .Where(d => d.Firstname == "John")
                .Execute().Should().Be(1);
            _db.CountRows<User>(d => d.FirstName != "John3").Should().Be(2);         
        }

        public void Dispose()
        {
           _db.Dispose();
        }
    }

    public class SqlServerTests : ADBOperationsTests
    {
        public static string ConnectionString =>
            Setup.IsAppVeyor
                ? @"Server=(local)\SQL2016;Database=tempdb;User ID=sa;Password=Password12!"
                : @"Data Source=.\SQLExpress;Initial Catalog=tempdb;Integrated Security=True;MultipleActiveResultSets=True";

        public SqlServerTests() : base()
        {

        }
    }
}