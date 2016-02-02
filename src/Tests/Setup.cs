using System;
using System.Data.Common;
using System.Linq;
using CavemanTools.Logging;
using CavemanTools.Model.ValueObjects;
using CavemanTools.Testing;
using SqlFu.Builders;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;
using SqlFu.Mapping.Internals;
using SqlFu.Providers.SqlServer;
using SqlFu.Tests._Fakes;

namespace SqlFu.Tests
{
    public class Setup
    {
        public const string Connex = @"Data Source=.\SQLExpress;Initial Catalog=tempdb;Integrated Security=True;MultipleActiveResultSets=True;Asynchronous Processing=True";

        static Setup()
        {
            

        }
        public static ConvertersManager Converters()
        {
            var r= new ConvertersManager();
            r.RegisterConverter(o=> o.As<string>().IsNullOrEmpty()?null:new Email(o.ToString()));
            return r;
        }

    
        public static DbConnection GetConnection()
        {
            SqlFuManager.UseLogManager();
            return SqlFuManager.OpenConnection(SqlServer2012Provider.Instance, Connex);
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
            data["Name"] = "bla";
            data["Version"] = new byte[] {0, 1};
            data["Id"] = Guid.Empty;
            data["Decimal"] = 34;
            data["Email"] = DBNull.Value;
            data["Address"] = "";
            data["Author_Id"] = DBNull.Value;
            data["Author_Name"] = "AuthorName";
            if (config!=null)config(data);
            return data;
        }

        public static CustomMappers UserMappers()
        {
            var r = new CustomMappers();
            r.Register(d=> new Address(d["Address"] as string));
            return r;
        }


        public static MapperFactory MapperFactory()
        {
            return new MapperFactory(UserMappers(),InfoFactory(),new ConvertersManager());
        }

        public static TableInfoFactory InfoFactory()
        {
            return new TableInfoFactory(Converters());
        }
    }
}