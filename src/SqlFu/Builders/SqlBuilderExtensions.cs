using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SqlFu.Builders
{
    public static class SqlBuilderExtensions
    {
        /// <summary>
        /// Placeholder method to represent a "column in (list)" scenario in sql builder.
        /// Use it _only_ inside an expression.
        /// </summary>
        /// <example>
        /// var values=new[]{val1,val2};
        ///  [..]Where(post=>post.Title.HasValueIn(values))
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="column">Column</param>
        /// <param name="values">The collection of values</param>
        /// <returns></returns>
        public static bool HasValueIn<T>(this T column, IEnumerable<T> values)
        {
            throw new NotImplementedException("This shouldn't be called directly");
        }


        /// <summary>
        /// Placeholder method to injects raw sql into a strongly typed query
        /// It should be used _only_ inside an expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="table"></param>
        /// <param name="sql">Raw sql string with placeholders for named parameters 'where my id=@id'</param>
        /// <param name="args">Anonymous object with the values of the parameters</param>
        /// <example>
        /// [..]Where(d=>d.InjectSql("my id=@id",new{id=2}))
        /// </example>
        /// <returns></returns>
        public static R InjectSql<T, R>(this T table, string sql, R args)
        {
            throw new NotImplementedException("This shouldn't be called directly");
        }

        /// <summary>
        /// Sql count(*)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static int Count<T>(this T table)
        {
            throw new NotImplementedException("This shouldn't be called directly");
        }

        /// <summary>
        /// Sql count(column)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static int Count<T,R>(this T table,R expression)
        {
            throw new NotImplementedException("This shouldn't be called directly");
        }


        /// <summary>
        /// Sql sum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <example>
        /// Where(table=>table.sum(table.id))
        /// </example>
        /// <returns></returns>
        public static T Sum<T,R>(this T table,R expression)
        {
            return default(T);
        }

        /// <summary>
        /// Sql max
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T Max<T,R>(this T table, R expression)
        {
            return default(T);
        }
         /// <summary>
        /// Sql min
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T Min<T,R>(this T table, R expression)
        {
            return default(T);
        }
        
        /// <summary>
        /// Sql floor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T Floor<T,R>(this T table, R expression)
        {
            return default(T);
        }


        /// <summary>
        /// Sql Ceiling
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T Ceiling<T,R>(this T table, R expression)
        {
            return default(T);
        }

       /// <summary>
        /// Sql avg
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T Avg<T,R>(this T table, R expression)
        {
            return default(T);
        }


        /// <summary>
        /// Sql round
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T Round<T, R>(this T table, R expression,int length)
        {
            return default(T);
        }


        public static string Concat<T>(this T table, params object[] expressions)
        {
            throw new NotImplementedException();
            
        }
        //public static void InjectSql(string sql)
    }
}