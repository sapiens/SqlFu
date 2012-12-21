using System.Text;
using SqlFu.DDL.Internals;
using SqlFu.Providers.SqlServer;

namespace SqlFu.DDL.Generators.SqlServer
{
    internal class SqlServerIndexWriter : AbstractIndexWriter
    {
        public SqlServerIndexWriter(StringBuilder builder) : base(builder, DbEngine.SqlServer)
        {
        }


        protected override void WriteEndOptions()
        {
            WriteIncludes(Index.Options);
            WriteWith(Index.Options);
        }

        protected override void WriteIndexName()
        {
            Builder.Append(SqlServerProvider.EscapeIdentifier(Index.Name));
        }

        protected override void WriteTableName()
        {
            Builder.Append(SqlServerProvider.EscapeIdentifier(Index.TableName));
        }

        protected override void WriteColumn(IndexColumn column)
        {
            Builder.Append(SqlServerProvider.EscapeIdentifier(column.Name));
        }

        protected override void WriteIndexType()
        {
            base.WriteIndexType();
            var option = Index.Options.Get(SqlServerOptions.Clustered);
            if (option != null)
            {
                Builder.AppendFormat(" {0}", option);
            }
            else
            {
                option = Index.Options.Get(SqlServerOptions.NonClustered);
                if (option != null)
                {
                    Builder.Append(" " + option);
                }
            }
        }

        private void WriteWith(DbEngineOptions options)
        {
            if (options.HasAny(SqlServerOptions.Drop_Existing, SqlServerOptions.Ignore_Dup_Key))
            {
                Builder.Append(" WITH (");

                var ignore = options.Get(SqlServerOptions.Ignore_Dup_Key);
                if (ignore != null)
                {
                    Builder.AppendFormat("{0},", ignore);
                }

                var drop = options.Get(SqlServerOptions.Drop_Existing);
                if (drop != null)
                {
                    Builder.AppendFormat(" {0},", drop);
                }
                Builder.RemoveLast();
                Builder.Append(")");
            }
        }

        private void WriteIncludes(DbEngineOptions options)
        {
            var opt = options.Get(SqlServerOptions.Include);
            if (opt != null) Builder.Append(" " + opt);
        }
    }
}