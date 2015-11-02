using System;
using System.Data.Common;
using CavemanTools.Logging;
using FluentAssertions;
using SqlFu.Configuration.Internals;
using Xunit;

namespace SqlFu.Tests.Providers.MsSql
{

    public class MyCreateDropPoco
    {
        public Guid Id { get; set; }        
    }
    
    public class MssqlCreateTableBuilderTests:IDisposable
    {
        private DbConnection _db;

        public MssqlCreateTableBuilderTests()
        {
            LogManager.OutputToConsole();
            SqlFuManager.UseLogManager();
            _db = Setup.GetConnection();
        }

        //[Fact]
        //public void get_sql()
        //{
        //    var map = new BuilderMapping();
        //    var sut = new MsSqlTableBuilder(SqlServer2012Provider.Instance as SqlServer2012Provider);
        //    var result = sut.GetTableCreateSql(map.Info);
        //    Console.WriteLine(result);
        //}


        //[Fact]
        //public void create_table_then_drop_it()
        //{
        //    _db.CreateTable<MyCreateDropPoco>();
        //    var exists = _db.TableExists<MyCreateDropPoco>();
        //    exists.Should().BeTrue();
        //    _db.DropTable<MyCreateDropPoco>();
        //    exists = _db.TableExists<MyCreateDropPoco>();
        //    exists.Should().BeFalse();
        //}

        //[Fact]
        //public void second_create_throws()
        //{
        //    _db.CreateTable<MyCreateDropPoco>();
        //    _db.Invoking(d => d.CreateTable<MyCreateDropPoco>(IfTableExists.Throw)).ShouldThrow<TableExistsException>();
        //}
        
        //[Fact]
        //public void second_create_is_ignored()
        //{
        //     _db.CreateTable<MyCreateDropPoco>(IfTableExists.Ignore);
        //    _db.Invoking(d => d.CreateTable<MyCreateDropPoco>()).ShouldNotThrow<TableExistsException>();
        //    var exists =_db.TableExists<MyCreateDropPoco>();
        //    exists.Should().BeTrue();
        //}
        
        //[Fact]
        //public void second_create_drops_existing()
        //{
        //    _db.CreateTable<MyCreateDropPoco>(IfTableExists.DropIt);
        //    _db.Invoking(d => d.CreateTable<MyCreateDropPoco>()).ShouldNotThrow<TableExistsException>();
        //    var exists =_db.TableExists<MyCreateDropPoco>();
        //    exists.Should().BeTrue();
        //}

        public void Dispose()
        {
            _db.DropTable<MyCreateDropPoco>();
            _db.Dispose();
        }
    }
}