using System;
using System.Linq.Expressions;
using System.Text;

namespace SqlFu.Builders
{
   
    
    public class UpdateTableBuilder<T>:ASqlBuilder,IBuildUpdateTable<T>
    {
        public UpdateTableBuilder(SqlQueryManager manager) : base(manager)
        {
            manager.NoAliases = true;
            Builder.AppendFormat("update {0} set ", manager.FormatTableName(typeof (T)));
        }

        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, Expression<Func<T, object>> statement)
        {
            var writer = ExpressionWriter;
            writer.Write(column);
            Builder.Append("=");
            writer.Write(statement);
            Builder.Append(",");
            return this;
        }

        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, object value)
        {
            var writer = ExpressionWriter;
            writer.Write(column);
            Builder.Append("=");
            Builder.Append("@"+Parameters.CurrentIndex);
            Parameters.AddValue(value);
            Builder.Append(",");
            return this;
        }

        public IBuildUpdateTable<T> Set(string propertyName, object value)
        {
            Builder.AppendFormat("{0}={1},", Manager.GetColumnName(propertyName, typeof(T)),"@"+Parameters.CurrentIndex);
            Parameters.AddValue(value);
            return this;
            
        }

        public IBuildUpdateTable<T> Where(Expression<Func<T, bool>> criteria)
        {
            criteria.MustNotBeNull();
            
            Parts.Add(Where_,criteria);
            return this;
        }

        protected override void BuildParts()
        {
            Builder.RemoveLastIfEquals(',');
            WriteCriteria();
        }
    }
}