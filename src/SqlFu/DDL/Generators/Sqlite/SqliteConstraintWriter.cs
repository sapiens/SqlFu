namespace SqlFu.DDL.Generators.Sqlite
{
    //internal abstract class SqliteConstraintWriter : AbstractConstraintWriter
    //{
    //    public SqliteConstraintWriter(StringBuilder builder)
    //        : base(builder, DbEngine.SQLite)
    //    {
    //    }

    //    protected override string ConstraintName(string name)
    //    {
    //        return SqliteProvider.EscapeIdentifier(name);
    //    }

    //    protected override void WriteColumnsNames(string columns, StringBuilder builder)
    //    {
    //      //  SqliteDDLWriter.WriteColumnsNames(columns, builder);
    //    }

    //}

    //class SqliteUniqueKeyWriter:SqliteConstraintWriter
    //{
    //    public SqliteUniqueKeyWriter(StringBuilder builder) : base(builder)
    //    {
    //    }

    //    public void Write(UniqueKeyConstraint key)
    //    {
    //        base.Write(key);
    //        var opt = key.Options.Get(SqliteOptions.On_Conflict);
    //        if (opt!=null)
    //        {
    //            Builder.Append(" " + opt);
    //        }
    //    }

    //    protected override void WriteConstraintType()
    //    {
    //        var key = Definition as UniqueKeyConstraint;
    //        var name=key.IsPrimary?"PRIMARY KEY":"UNIQUE";
    //        Builder.Append(name);
    //    }
    //}

    //class SqliteCheckWriter:SqliteConstraintWriter
    //{
    //    public SqliteCheckWriter(StringBuilder builder) : base(builder)
    //    {

    //    }

    //    protected override void WriteConstraintType()
    //    {
    //        Builder.Append("CHECK");
    //    }

    //    protected override void WriteConstraintFeature()
    //    {
    //        var key = Definition as CheckConstraint;
    //        Builder.Append(key.Expression);
    //    }
    //}
}