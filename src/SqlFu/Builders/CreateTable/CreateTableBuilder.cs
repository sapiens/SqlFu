using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using SqlFu.Configuration;
using SqlFu.Providers;

namespace SqlFu.Builders.CreateTable
{
    public class CreateTableBuilder
    {
        private readonly IDbProvider _provider;
        StringBuilder _sb=new StringBuilder();
        private TableCreationData _data;
        private string _tableName;

        public CreateTableBuilder(IDbProvider provider)
        {
            _provider = provider;
            
        }
        
        public string GetSql(TableCreationData data)
        {
            _data = data;
            _tableName = _provider.EscapeTableName(data.TableName);
           _provider.SqlFuConfiguration
                .TableInfoFactory.GetInfo(data.Type)
                .Table=data.TableName;
            _sb.AppendLine("create table " + _tableName + "(");
            AddColumns(data.Columns);
            AddConstraints(data.PrimaryKey);
            AddConstraints(data.ForeignKeys);
            _sb.Append(");");
            AddIndexes(data.Indexes);
            return _sb.ToString();
        }

        private void AddIndexes(List<IndexDefinition> indexes)
        {
            var sb=new StringBuilder();

            indexes.ForEach(idx =>
            {
                sb.Clear();
                sb.Append("create ");
                if (idx.IsUnique) sb.Append("unique ");
                sb.Append("index ");
                sb.Append(idx.Name ?? "ix_" + _data.TableName.DDLUsableString + "_" + DateTime.Now.Ticks);
                sb.Append($" on {_tableName}(");
                idx.Columns.ForEach(n =>
                {
                    sb.Append(n + ",");
                });
                sb.RemoveLast().Append(")");
                sb.AppendLine(";");
                _sb.Append(_provider.FormatIndexOptions(sb.ToString(), idx.Options));
            });
        }

        private void AddConstraints(List<ForeignKeyDefinition> foreignKeys)
        {
            _sb.RemoveLastIfEquals(',').Append(',');
            foreignKeys.ForEach(fk =>
            {
                _sb.Append("foreign key (");
                fk.Columns.ForEach(col => _sb.Append(_provider.EscapeIdentifier(col) + ","));
                _sb.RemoveLast().Append(")");
                _sb.Append(" references " + _provider.EscapeTableName(fk.ParentTable) + "(");
                fk.ParentColumns.ForEach(col => _sb.Append(_provider.EscapeIdentifier(col) + ","));
                _sb.RemoveLast().Append(")");
                if (fk.OnUpdate != ForeignKeyRelationCascade.NotSet)
                {
                    _sb.Append(" on update " + fk.OnUpdate);
                }
                if (fk.OnDelete != ForeignKeyRelationCascade.NotSet)
                {
                    _sb.Append(" on delete " + fk.OnDelete);
                }
                _sb.AppendLine().Append(',');
            });
            _sb.RemoveLast();
        }

        private void AddConstraints(PKData primaryKey)
        {
            if (primaryKey == null) return;
            var sb=new StringBuilder();

            var name = primaryKey.Name.IsNullOrEmpty()?"pk_" + _data.TableName.DDLUsableString:primaryKey.Name;

            sb.Append($"constraint {name} primary key (");
            primaryKey.Columns.ForEach(c =>
            {
                sb.Append(_provider.EscapeIdentifier(c) + ",");
            });
            sb.RemoveLast();
            sb.AppendLine(")");

            _sb.AppendLine(","+_provider.FormatIndexOptions(sb.ToString(), primaryKey.Options));
        }

        private void AddColumns(List<ColumnDefinition> columns)
        {
           columns.ForEach(col =>
           {
               _sb.AppendLine();
               if (col.Definition.IsNotEmpty())
               {
                   _sb.Append(col.Definition + ",");
                   return;
               }

               var name = _provider.EscapeIdentifier(col.PropertyName);
               var size = col.Size ?? "";

               var collation = col.Collation ?? "";

               var def = col.DefaultValue==null?"":$"default ({col.DefaultValue})";

               var isnull = col.IsNull ? "null" : "not null";

               var identity = col.IsIdentity? _provider.GetIdentityKeyword():"";
               _sb.Append($"{name} {col.DbType}{size} {collation} {def} {isnull} {identity},");
           });
            _sb.RemoveLast().AppendLine();
        }

        
     

        
    }
}