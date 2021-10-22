using SqlFu;
using SqlFu.Builders.Expressions;
using SqlFu.Configuration.Internals;
using SqlFu.Executors;
using SqlFu.Mapping.Internals;
using SqlFu.Providers;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Tests._Fakes;
using Tests.TestData;
using Tests.Usage;

namespace Tests
{
	public class Setup
	{


		static Setup()
		{


		}

		public static DbConnection SqlFuConnection(DbProvider provider, string cnx, Action<SqlFuConfig> config = null)
		{
			var c = new SqlFuConfig();
			c.LogWith(w => Debug.WriteLine(w));

			c.WhenType<ArticleType>().WriteAs(a => a.ToString());
			c.ConfigureTableForPoco<User>(d =>
			{

				d.TableName = "Users" + new string(Guid.NewGuid().ToByteArray().ToBase64().Where(w => (w >= 'a' && w <= 'z')).Take(5).ToArray());
				d.Property(f => f.Id).IsAutoincremented();
				d.IgnoreProperties(f => f.Ignored);

			});
			config?.Invoke(c);
			return new SqlFuConnection(provider, cnx, c);
		}

		public static FakeWriter FakeWriter() => new FakeWriter();

		public static readonly bool IsAppVeyor =
			Environment.GetEnvironmentVariable("Appveyor")?.ToUpperInvariant() == "TRUE";


		public static ConvertersManager Converters()
		{
			var r = new ConvertersManager();

			//   r.MapValueObject(e=>e.Value,o=> o.CastAs<string>().IsNullOrEmpty()?null:new Email(o.ToString()));
			return r;
		}


		public static FakeReader FakeReader(Action<FakeReader> config = null)
		{
			var data = new FakeReader();
			data.Add("Name", "bla");
			data.Add("Version", new byte[] { 0, 1 });
			data.Add("Id", Guid.Empty);
			data.Add("Decimal", 34);
			data.Add("Email", DBNull.Value);
			data.Add("Address", "");
			config?.Invoke(data);
			return data;
		}

		public static CustomMappersConfiguration UserMappers()
		{
			var r = new CustomMappersConfiguration();
			r.Register(d => new Address(d["Address"] as string));
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
			return new MapperFactory(UserMappers(), InfoFactory(), Converters());
		}

		public static TableInfoFactory InfoFactory() => new TableInfoFactory(Converters());

		public static ExpressionSqlGenerator CreateExpressionSqlGenerator(IDbProviderExpressions exprProvider)
		{
			return new ExpressionSqlGenerator(exprProvider, Setup.InfoFactory(), new FakeEscapeIdentifier());
		}

		public static string SqlServerConnection =>
			IsAppVeyor
				? @"Server=(local)\SQL2017;Database=tempdb;User ID=sa;Password=Password12!"
				: @"Data Source=(localdb)\ProjectsV13;Initial Catalog=tempdb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

		public static string SqliteConnection { get; } = "Data Source=test.db;Version=3;New=True;BinaryGUID=False";
	}
}