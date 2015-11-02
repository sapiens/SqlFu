using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SqlFu.Builders.Internals.SqlParts;

namespace SqlFu.Builders
{
   public class DeleteSqlBuilder<T> : ASqlBuilder,IDeleteWhereConnector<T>,IDeleteFromSubQuery<T>
   {
        public DeleteSqlBuilder(SqlQueryManager manager):base(manager)
        {
            manager.NoAliases = true;
            Builder.Append("delete");
        }

        public IDeleteWhereConnector<T> Where(Expression<Func<T, bool>> criteria)
        {
            if (criteria == null) return this;
            Parts.Add(Where_,criteria);
            return this;
        }

       private LambdaExpression _currentColumn;

       public IDeleteFromSubQuery<T> Where(Expression<Func<T, object>> column)
       {
           _currentColumn = column;
           return this;
       }

       protected override void BuildParts()
        {
            Parts.GetParts(s=>s==PreFrom).ForEach(v=>Builder.AppendFormat("{0},",v.ToString()));
            Builder.RemoveLastIfEquals(',');
            WriteFrom<T>();
            WriteCriteria();
        }

       public IDeleteWhere<T> And
       {
           get
           {
               Parts.Add(new SqlBuilderPartValue(WhereAnd,"and"));
               return this;
           }
       }

       public IDeleteWhere<T> Or
       {
           get
           {
               Parts.Add(WhereOr, "or");
               return this;
           }
       }

       private int _subq = 0;

       public IDeleteWhereConnector<T> In(IBuildSql subquery)
       {
           _subq++;
           Parts.Add(new SqlBuilderPartSubQuery(new SqlBuilderPartValue(WhereIn,"in"),_currentColumn,subquery, _subq,ExpressionWriter,Parameters));
           _currentColumn = null;
           return this;
       }

       public IDeleteWhereConnector<T> NotIn(IBuildSql subquery)
       {
           _subq++;
           Parts.Add(new SqlBuilderPartSubQuery(new SqlBuilderPartValue(WhereNotIn, "not in"), _currentColumn, subquery, _subq, ExpressionWriter, Parameters));
           _currentColumn = null;
           return this;
       }
   }
}