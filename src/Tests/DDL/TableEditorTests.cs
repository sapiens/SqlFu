using System.Data;
using Moq;
using SqlFu;
using SqlFu.DDL.Generators;
using SqlFu.DDL.Generators.SqlServer;
using SqlFu.DDL.Internals;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL
{
    public class TableEditorTests
    {
        private Stopwatch _t = new Stopwatch();
        private ModifyTableBuilder _table;

        class DDLStub:IGenerateDDL
        {
            public string GenerateCreateTable(TableSchema table)
            {
                throw new NotImplementedException();
            }

            public string GenerateAlterTable(TableSchema schema)
            {
                throw new NotImplementedException();
            }
        }

        public TableEditorTests()
        {
           
            _table = new ModifyTableBuilder(Setup.GetDb(),new DDLStub(), "test");
        }

       

        [Fact]
        public void dropping_elements()
        {
            _table.Columns.Drop("id").WithoutOptions()
                .Constraints.Drop("fk").WithoutOptions()
                .Indexes.Drop("idx");
            
            Assert.Equal(1,_table.Table.ModifiedColumns.DroppedColumns.Count);
            Assert.Equal(1,_table.Table.Constraints.Dropped.Count);
            Assert.Equal(1,_table.Table.Indexes.Dropped.Count);
        }

        [Fact]
        public void rename_column()
        {
            _table.Columns.RenameColumn("id", "nid");
            Assert.Equal("nid",_table.Table.ModifiedColumns["id"].Modifications.NewName);
        }

        [Fact]
        public void add_column()
        {
            _table.Columns.Add("id", DbType.Int32);
            Assert.NotNull(_table.Table.Columns["id"]);
        }

        [Fact]
        public void modify_column()
        {
            _table.Columns.Change("id")
                .SetDefault("1")
                .SetNullable(false);
               
            var mod = _table.Table.ModifiedColumns["id"].Modifications;
            Assert.False(mod.DefaultDropped);
            Assert.Equal("1",mod.DefaultValue);
            Assert.False(mod.Nullable.Value);
        }

        [Fact]
        public void drop_column_default()
        {
            _table.Columns.Change("id").DropDefault();
            Assert.True(_table.Table.ModifiedColumns["id"].Modifications.DefaultDropped);
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}