using System;
using System.Data.Common;
using CavemanTools.Model.ValueObjects;
using SqlFu;
using SqlFu.Builders.Expressions;
using SqlFu.Configuration.Internals;
using SqlFu.Mapping.Internals;
using SqlFu.Providers;
using Tests.TestData;
using Tests._Fakes;

namespace Tests
{
    public class Setup
    {
        

        static Setup()
        {
            

        }

        public static DbConnection SqlFuConnection(string cnx)
        {
           
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
    }
}