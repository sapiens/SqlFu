using System;
using System.Linq;
using CavemanTools.Logging;
using CavemanTools.Model.ValueObjects;
using FakeItEasy;
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

        public static FakeWriter FakeWriter() => new FakeWriter();
      



        public static ConvertersManager Converters()
        {
            var r= new ConvertersManager();

            r.MapValueObject(e=>e.Value,o=> o.CastAs<string>().IsNullOrEmpty()?null:new Email(o.ToString()));
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

        public static TableInfo GetTableInfo<T>()=>new TableInfo(typeof(T),Converters());


        public static MapperFactory MapperFactory()
        {
            return new MapperFactory(UserMappers(),InfoFactory(),Converters());
        }

        public static TableInfoFactory InfoFactory() => new TableInfoFactory(Converters());
    }
}