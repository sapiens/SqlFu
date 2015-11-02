using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlFu.Builders.Internals;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;
using SqlFu.Providers;

namespace SqlFu.Builders
{
    public abstract class AbstractTableBuilder:IBuildSqlToCreateTable
    {
        private readonly IDbProvider _provider;
        private StringBuilder _sb;

        protected AbstractTableBuilder(IDbProvider provider)
        {
            _provider = provider;
            _sb = new StringBuilder();
        }

        protected StringBuilder Builder
        {
            get { return _sb; }
        }

        protected void WriteCreateTable()
        {
            _sb.Append("create table ");
        }

        protected void WriteTableName()
        {
            //if (!ListUtils.IsNullOrEmpty(schema))
            //{
            //    _sb.AppendFormat(_provider.EscapeIdentifier(schema)).Append(".");
            //}
            _sb.Append(_info.EscapeName(_provider));
        }


        protected void WriteColumns(IEnumerable<ColumnInfo> columns)
        {
           Builder.WriteColumns(columns,_provider);           
        }

      

        protected const string PrimaryKey = "primary key";
        protected const string ForeignKey = "foreign key";
        protected const string Check = "check";

        protected void WriteConstraintName(string name, string type)
        {
            Builder.Append("constraint ").Append(_provider.EscapeIdentifier(name)).Append(" "+type);
        }
      
        protected void WriteColumnDefinition(ColumnInfo info,Action<IWriteColumnDefinition> config)
        {
            using (var col = new Column(info, _sb, _provider))
            {
                if (!info.Features[_provider.ProviderId].Redefined.IsNullOrEmpty())
                {
                    _sb.Append(info.Features[_provider.ProviderId].Redefined);
                    return;
                }
                config(col);
            }
        }

        protected  TableInfo _info;

        protected void WriteCheck(Check ck)
        {
            ck.Name = ck.Name ?? "Ck_" + _info.Name + Guid.NewGuid();
            WriteConstraintName(ck.Name, Check);
            Builder.AppendFormat("({0})\n,", ck.Expression);
        }

        protected void WriteForeignKey(ForeignKey fk)
        {
            var refTable = fk.ReferencedColumns.First().Table;
            fk.Name = fk.Name ?? "FK_{0}_{1}".ToFormat(_info.Name, refTable.Name);
            WriteConstraintName(fk.Name, ForeignKey);
            WriteColumns(fk.Columns);
            Builder.AppendFormat(" references {0}",refTable.EscapeName(_provider));
            WriteColumns(fk.ReferencedColumns);
            Builder.AppendFormat(" on delete {0} on update {1}", ToString(fk.OnDelete), ToString(fk.OnUpdate));
            Builder.AppendLine().Append(",");
        }

        protected IWriteIndex GetIndexWriter()
        {
            return new IndexWriter(_sb, _provider);
        }

        public static string ToString(ForeignKeyRelationCascade relation)
        {
            switch (relation)
            {
                case ForeignKeyRelationCascade.Cascade:
                    return "CASCADE";
                case ForeignKeyRelationCascade.Restrict:
                    return relation.ToString();
                case ForeignKeyRelationCascade.SetDefault:
                    return "SET DEFAULT";
                case ForeignKeyRelationCascade.SetNull:
                    return "SET NULL";
                default:
                    return "NO ACTION";
            }
        }

        public abstract string GetTableCreateSql(TableInfo info);

    }
}