using System;
using System.Linq;
using CavemanTools.Logging;
using CavemanTools.Model.ValueObjects;
using CavemanTools.Testing;
using FakeItEasy;
using SqlFu.Builders.Expressions;
using SqlFu.Configuration.Internals;
using SqlFu.Mapping.Internals;
using SqlFu.Providers;
using Tests.Data;
using Tests._Fakes;

namespace Tests
{

    /*  "dnxcore50": {
            "dependencies": {
                "System.Data.Common": "4.0.1-*",
                "Microsoft.CSharp": "4.0.1-beta-23516",
                "System.Collections": "4.0.11-beta-23516",
                "System.Linq": "4.0.1-beta-23516",
                "System.Runtime": "4.0.21-beta-23516",
                "System.Threading": "4.0.11-beta-23516"            
            }
        }*/
    public class Setup
    {
        public const string Connex = @"Data Source=.\SQLExpress;Initial Catalog=tempdb;Integrated Security=True;MultipleActiveResultSets=True;Asynchronous Processing=True";

        static Setup()
        {
            

        }

        public static IGenerateSqlFromExpressions FakeWriter() =>  A.Fake<IGenerateSqlFromExpressions>();
      



        public static ConvertersManager Converters()
        {
            var r= new ConvertersManager();
            r.RegisterConverter(o=> o.As<string>().IsNullOrEmpty()?null:new Email(o.ToString()));
            return r;
        }

    public static void DoBenchmark(int iterations=500, params BenchmarkAction[] actions)
        {
            "benchmark".LogInfo("Starting");
            var b = new BenchmarksContainer();
            b.Iterations = iterations;
            b.Add(j => { }, "nothing");
            var i = 1;
            foreach (var a in actions)
            {
                b.Add(a,"option "+i);
                i++;
            }
            b.ExecuteWarmup();
            b.Execute();
            b.GetResults.ForEach(r=>"benchmark".LogInfo(r.ToString()));
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