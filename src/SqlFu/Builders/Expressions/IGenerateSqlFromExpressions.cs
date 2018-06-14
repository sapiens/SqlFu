using System;
using System.Linq.Expressions;

namespace SqlFu.Builders.Expressions
{
    public interface IGenerateSqlFromExpressions
    {
     
        ParametersManager Parameters { get; }
    
      bool EscapeIdentifiers { get; set; }
     
        /// <summary>
        /// Use it when you need to generate only columns for select,order by, group by
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        string GetColumnsSql(params Expression[] columns);
    
       
        /// <summary>
        /// Use this to generate criteria
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        string GetSql(Expression expression);

    }
}