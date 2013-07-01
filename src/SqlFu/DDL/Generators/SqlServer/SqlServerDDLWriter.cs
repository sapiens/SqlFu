using System.Text;
using SqlFu.DDL.Internals;
using SqlFu.Providers.SqlServer;

namespace SqlFu.DDL.Generators.SqlServer
{
    internal class SqlServerDDLWriter : CommonDDLWriter
    {
        public SqlServerDDLWriter(SqlFuConnection db) : base(db, DbEngine.SqlServer)
        {
        }

        protected override void WriteTableName()
        {
            Builder.Append(SqlServerProvider.EscapeIdentifier(Table.Name));
        }


        protected override AbstractColumnWriter GetColumnWriter()
        {
            return new SqlServerColumnWriter(Builder);
        }

        protected override AbstractUniqueKeyWriter GetUniqueKeyWriter()
        {
            return new SqlServerUniqueKeyWriter(Builder);
        }

        protected override AbstractCheckWriter GetCheckWriter()
        {
            return new SqlServerCheckWriter(Builder);
        }

        protected override AbstractForeignKeyWriter GetForeignKeyWriter()
        {
            return new SqlServerForeignKeyWriter(Builder);
        }

        protected override AbstractIndexWriter GetIndexWriter()
        {
            return new SqlServerIndexWriter(Builder);
        }


        public static void WriteColumnsNames(string columns, StringBuilder builder)
        {
            WriteColumnsNames(columns, builder, SqlServerProvider.EscapeIdentifier);
        }

        //public static void WriteColumnsNames(ICollection<string> columns,StringBuilder builder)
        //{
        //    WriteColumnsNames(columns,builder,SqlServerProvider.EscapeIdentifier);            
        //}

        #region Alter Table

        protected override AbstractChangedColumnsManager GetChangedColumnsManager()
        {
            return new SqlServerChangedColumnsManager(Builder, Db);
        }

        protected override void WriteRenameColumn(ColumnModifications col)
        {
            Builder.AppendFormat("exec sp_rename '{2}.{0}','{1}','COLUMN';\n", col.Current.Name, col.NewName, Table.Name);
        }

        protected override AbstractDropIndexWriter GetDropIndexWriter()
        {
            return new SqlServerDropIndexWriter(Builder);
        }

        protected override AbstractDropConstraintWriter GetDropConstraintWriter()
        {
            return new SqlServerDropConstraintWriter(Builder, Db.DatabaseTools);
        }

        protected override AbstractDropColumnWriter GetDropColumnWriter()
        {
            return new SqlServerDropColumnWriter(Builder);
        }

        protected override string Escape(string name)
        {
            return SqlServerProvider.EscapeIdentifier(name);
        }


        protected override string GetAddConstraintPrefix()
        {
            return "";
        }

        #region Sql future use

        /*
         * Find index
         * 
         * SELECT 
     ind.name    
		,t.name as tableName  
    ,col.name     
FROM sys.indexes ind 

INNER JOIN sys.index_columns ic 
    ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 

INNER JOIN sys.columns col 
    ON ic.object_id = col.object_id and ic.column_id = col.column_id 

INNER JOIN sys.tables t 
    ON ind.object_id = t.object_id 

WHERE (1=1) 
    AND ind.is_primary_key = 0 
    AND ind.is_unique = 1 
    AND ind.is_unique_constraint = 0 
    AND t.is_ms_shipped = 0 
ORDER BY 
    t.name, ind.name
         * 
         * For Pk,Fk ,Checks  
         * select * from [INFORMATION_SCHEMA].[TABLE_CONSTRAINTS]
         * */

        #endregion

        #endregion
    }
}