using System;
using System.Data.Common;
using DomainBus.Tests;
using FluentAssertions;
using SqlFu.Builders.CreateTable;
using SqlFu.Configuration.Internals;
using SqlFu.Tests._Fakes;
using Xunit;
using Xunit.Abstractions;

namespace SqlFu.Tests.Builders
{
    public class UpdateBuilderTests:IDisposable
    {
        private DbConnection _db;

        public UpdateBuilderTests(ITestOutputHelper x)
        {
            x.Logger();
            _db = Setup.GetConnection();
            _db.CreateTableFrom<SomeData>(tc =>
            {
                tc.IfTableExists(IfTableExists.DropIt);
                tc.ColumnSize(d => d.Name, 50);
            });
        }

        [Fact]
        public void set_fields()
        {
            _db.Insert(new SomeData() {Id=2,Name="34"});

            _db.Update<SomeData>().Set(d => d.Counter, d => d.Counter + 1).Set(d=>d.Name,"ch")
                .Set(d=>d.Time,DateTime.UtcNow)
                .Execute();

            _db.GetQueryValue(t => t.From<SomeData>().Where(d => d.Id == 2).Select(d => d.Counter)).Should().Be(1);
            var rez = _db.GetSingle<SomeData>(t => t.Id == 2);
            rez.Name.Should().Be("ch");

        }

        public void Dispose()
        {
           _db.Dispose();            
        }
    }
}