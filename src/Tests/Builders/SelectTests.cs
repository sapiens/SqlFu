//using System;
//using System.Data.Common;
//using System.Linq.Expressions;
//using FluentAssertions;
//using SqlFu.Builders.Expressions;
//using SqlFu.SqlServer;
//using SqlFu.Tests.Providers;
//using SqlFu.Tests._Fakes;
//using Xunit;

//namespace SqlFu.Tests.Builders
//{
//    public class SelectTests:IDisposable
//    {
//        class Joined
//        {
//            public string Name { get; set; }
//        }
        
//        private DbConnection _db;

//        public SelectTests()
//        {
//            var mapping = new BuilderMapping();
//            SqlFuManager.Config.AddTableMapping(mapping);
//            _db = Setup.GetConnection();
//            _db.CreateTable<SomePost>();
//        }
//       [Fact]
//        public void FactMethodName()
//       {
//           var id = 4;
//           var dec = 4m;
//           var sub = _db.From<MapperPost>().Select(d => d.Id).Where(d => d.Decimal == dec);
//           sub.GetSql();
//           var text = "g";
//           var func = SqlServer2012Provider.Instance.Functions;
//           var q = _db.From<SomePost>()
//                .Select(d => new{d.Id,Total=func.Sum(func.Min(d.Title)),Rd=func.Round(d.Id,id)})
//                .Where(d => d.Id == id)
//                .Or.Where(d=>func.FtsContains(text,d.Title,d.State))
//                //.And.Where(d=>d.Id).In(d=>d.From<MapperPost>().Select())
//                //.In(sub)
                

//                .GroupBy(d=>d.Id)
//                .Having(d=>func.Count(d.Id)>3)
//                .OrderBy(d=>d.Id)
//               // .OrderByDescending(d=>d.Total)
//               .Limit(10)
//                ;

//           Console.WriteLine(q.GetSql());
         
       

//        }

//        [Fact]
//        public void generate_columns_from_lambda()
//        {
//            var info = new BuilderMapping().Info;
//            var fac = Setup.InfoFactory();
//            fac.Add(info);
//            var ex = new ExpressionWriter(SqlServer2012Provider.Instance.GetExpressionsHelper(), Setup.SqlManager(fac));
//           // Expression<Func<SomePost,SomePostProjection>> cols=d=>new SomePostProjection(){Id=d.Id,Title=d.Title};
//            var func = SqlServer2012Provider.Instance.Functions;
//            var id = 4;
//            Expression<Func<SomePost, object>> c = d => new {d.Id,Title=func.Sum(func.Min(d.Id)),Fullname=func.Concat(d.Title," ",d.Title)};


//            var data = ex.GetSelectColumnsSql(c);
//            Console.WriteLine(data);
//        }

//        [Fact]
//        public void has_any_rows_with_condition()
//        {
//            var r= _db.HasAnyRows<SomePost>(d => d.Id == 2);
//            r.Should().BeFalse();
//        }
         
//        [Fact]
//        public void has_any_rows()
//        {
//            var r= _db.HasAnyRows<SomePost>();
//            r.Should().BeFalse();
//        }

//        public void Dispose()
//        {
//            _db.DropTable<SomePost>();
//            _db.Dispose();
//        }
//    }
//}