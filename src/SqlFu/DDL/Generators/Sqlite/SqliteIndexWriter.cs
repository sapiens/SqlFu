using System.Text;
using SqlFu.DDL.Internals;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.Sqlite
{
    internal class SqliteIndexWriter : AbstractIndexWriter
    {
        public SqliteIndexWriter(StringBuilder builder) : base(builder, DbEngine.SQLite)
        {
        }


        //protected override void WriteEndOptions()
        //{
        //    WriteIncludes(Index.Options);
        //    WriteWith(Index.Options);
        //}

        protected override void WriteIndexName()
        {
            Builder.Append(SqliteProvider.EscapeIdentifier(Index.Name));
        }

        protected override void WriteTableName()
        {
            Builder.Append(SqliteProvider.EscapeIdentifier(Index.TableName));
        }

        protected override void WriteColumn(IndexColumn column)
        {
            Builder.Append(SqliteProvider.EscapeIdentifier(column.Name));
        }

        //protected override void WriteIndexType()
        //{
        //    base.WriteIndexType();
        //    var option = Index.Options.Get(SqlServerOptions.Clustered);
        //    if (option != null)
        //    {
        //        Builder.AppendFormat(" {0}", option);
        //    }
        //    else
        //    {
        //        option = Index.Options.Get(SqlServerOptions.NonClustered);
        //        if (option != null)
        //        {
        //            Builder.Append(" " + option);
        //        }
        //    }
        //}

        //private void WriteWith(DbEngineOptions options)
        //{

        //    if (options.HasAny(SqlServerOptions.Drop_Existing,SqlServerOptions.Ignore_Dup_Key))
        //    {
        //        Builder.Append(" WITH (");

        //        var ignore = options.Get(SqlServerOptions.Ignore_Dup_Key);
        //        if (ignore!=null)
        //        {
        //            Builder.AppendFormat("{0},", ignore);                    
        //        }

        //        var drop = options.Get(SqlServerOptions.Drop_Existing);
        //        if (drop!=null)
        //        {
        //            Builder.AppendFormat(" {0},", drop);                                        
        //        }
        //        Builder.RemoveLast();
        //        Builder.Append(")");
        //    }
        //}

        //private void WriteIncludes(DbEngineOptions options)
        //{
        //    var opt = options.Get(SqlServerOptions.Include);
        //    if (opt!=null)Builder.Append(" "+opt.ToString());            
        //}
    }
}