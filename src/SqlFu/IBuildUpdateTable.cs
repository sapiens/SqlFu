using System;
using System.Data.Common;
using System.Linq.Expressions;
using SqlFu.Expressions;

namespace SqlFu
{
    public interface IBuildUpdateTable<T>
    {
        IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, Expression<Func<T, object>> statement);
        IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, object value);
        void Set(string column, object value);
        IBuildUpdateTable<T> Where(Expression<Func<T, bool>> criteria);
        int Execute();
    }

    internal class UpdateTableBuilder<T> : IBuildUpdateTable<T>
    {
        private readonly DbConnection _db;
        private readonly ExpressionSqlBuilder<T> _builder;

        public UpdateTableBuilder(DbConnection db)
        {
            db.MustNotBeNull();
            _db = db;
            _builder = new ExpressionSqlBuilder<T>(db.GetProvider().BuilderHelper);
            _builder.Append("update ").WriteTableName();
            _builder.Append(" set");
        }

        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, Expression<Func<T, object>> statement)
        {
            _builder.Append(" {0}=".ToFormat(_db.EscapeIdentifier(column.Body.GetPropertyName())));
            _builder.Write(statement);
            _builder.Append(",");
            return this;
        }

        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, object value)
        {
            Set(column.Body.GetPropertyName(), value);
            return this;
        }

        public void Set(string column, object value)
        {
            _builder.Append(" {0}=@{1},".ToFormat(_db.EscapeIdentifier(column), _builder.Parameters.CurrentIndex));
            _builder.Parameters.RegisterParameter(value);
        }

        public IBuildUpdateTable<T> Where(Expression<Func<T, bool>> criteria)
        {
            criteria.MustNotBeNull();
            _builder.EndEnumeration();
            _builder.Where(criteria);
            return this;
        }

        public string GetSql()
        {
            _builder.EndEnumeration();
            return _builder.ToString();
        }

        public object[] GetParameters()
        {
            return _builder.Parameters.ToArray();
        }

        public int Execute()
        {
            return _db.ExecuteCommand(_builder.ToString(), _builder.Parameters.ToArray());
        }
    }
}