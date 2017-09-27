using System;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using CavemanTools.Logging;
using CavemanTools.Model.ValueObjects;
using SqlFu;
using SqlFu.Builders.Expressions;
using SqlFu.Configuration.Internals;
using SqlFu.Executors;
using SqlFu.Mapping.Internals;
using SqlFu.Providers;
using Tests.SqlServer;
using Tests.TestData;
using Tests.Usage;
using Tests._Fakes;

namespace Tests
{
    public class Setup
    {
        

        static Setup()
        {
            

        }

        public static DbConnection SqlFuConnection(DbProvider provider,string cnx,Action<SqlFuConfig> config=null)
        {
            LogManager.OutputTo(w => Trace.WriteLine(w));
            
            var c=new SqlFuConfig();
            SqlFuManager.UseLogManager();
            c.ConfigureTableForPoco<User>(d =>
            {
                d.TableName = "Users" + new string(Guid.NewGuid().ToByteArray().ToBase64().Where(w=>(w>='a' && w<='z')).Take(5).ToArray());
                d.Property(f => f.Id).IsAutoincremented();
                d.IgnoreProperties(f=>f.Ignored);
                d.Property(f => f.Category).BeforeWritingUseConverter(t => t.ToString());
            });
            config?.Invoke(c);
            return new SqlFuConnection(provider,cnx,c);
        }

        public static FakeWriter FakeWriter() => new FakeWriter();

        public static readonly bool IsAppVeyor =
            Environment.GetEnvironmentVariable("Appveyor")?.ToUpperInvariant() == "TRUE";


        public static ConvertersManager Converters()
        {
            var r= new ConvertersManager();

         //   r.MapValueObject(e=>e.Value,o=> o.CastAs<string>().IsNullOrEmpty()?null:new Email(o.ToString()));
            return r;
        }

   
        public static FakeReader FakeReader(Action<FakeReader> config=null)
        {
            var data = new FakeReader();
            data.Add("Name", "bla");
            data.Add("Version", new byte[] { 0, 1 });
            data.Add("Id",Guid.Empty);
            data.Add("Decimal",34);
            data.Add("Email", DBNull.Value);
            data.Add("Address", "");
            config?.Invoke(data);
            return data;
        }

        public static CustomMappersConfiguration UserMappers()
        {
            var r = new CustomMappersConfiguration();
            r.Register(d=> new Address(d["Address"] as string));
            return r;
        }

        public static TableInfo GetTableInfo<T>()
        {
            var info = new TableInfo(typeof(T), Converters());
            if (typeof(T) == typeof(Post))
            {
                info.TableName = "SomePost";
                info["RegOn"].IgnoreWrite = true;
                //var sid = info["SomeId"];
                //sid.IgnoreWrite = sid.IgnoreRead = true;
            }
            
            return info;
        }


        public static MapperFactory MapperFactory()
        {
            return new MapperFactory(UserMappers(),InfoFactory(),Converters());
        }

        public static TableInfoFactory InfoFactory() => new TableInfoFactory(Converters());

        public static ExpressionSqlGenerator CreateExpressionSqlGenerator(IDbProviderExpressions exprProvider)
        {
          return new ExpressionSqlGenerator(exprProvider, Setup.InfoFactory(), new FakeEscapeIdentifier());
        }

        public static string SqlServerConnection =>
            IsAppVeyor
                ? @"Server=(local)\SQL2016;Database=tempdb;User ID=sa;Password=Password12!"
                : @"Data Source=.\SQLExpress;Initial Catalog=tempdb;Integrated Security=True;MultipleActiveResultSets=True";

        public static string SqliteConnection { get; } = "Data Source=:memory:;Version=3;New=True;BinaryGUID=False";
    }
}