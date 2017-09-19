using System;
using System.Collections.Generic;

namespace SqlFu
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
            throw new NotSupportedException("This shouldn't be called directly, only inside an expression of a sql builder or helper");
        }
         public static bool Between(this DateTime column, DateTime start,DateTime end)
        {
            throw new NotSupportedException("This shouldn't be called directly, only inside an expression of a sql builder or helper");
        }


        /// <summary>
        /// Placeholder method to injects raw sql into a strongly typed query.
        /// It should be used _only_ inside an expression when building a query
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
        public static bool InjectSql<T, R>(this T table, string sql, R args)
        {
            throw new NotSupportedException("This shouldn't be called directly, only inside an expression of a sql builder or helper");
        }

        /// <summary>
        /// Sql count(*)
        ///  It should be used _only_ inside an expression when building a query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static int Count<T>(this T table)
        {
            throw new NotSupportedException("This shouldn't be called directly, only inside an expression of a sql builder or helper");
        }

        /// <summary>
        /// Sql count(column)
        ///  It should be used _only_ inside an expression when building a query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static int Count<T,R>(this T table,R expression)
        {
            throw new NotSupportedException("This shouldn't be called directly, only inside an expression of a sql builder or helper");
        }


        /// <summary>
        /// Sql sum.
        ///  It should be used _only_ inside an expression when building a query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <example>
        /// Where(table=>table.sum(table.id))
        /// </example>
        /// <returns></returns>
        public static R Sum<T,R>(this T table,R expression)
        {
            throw new NotSupportedException("This shouldn't be called directly, only inside an expression of a sql builder or helper");
        }

        /// <summary>
        /// Sql max
        ///  It should be used _only_ inside an expression when building a query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static R Max<T,R>(this T table, R expression)
        {
            throw new NotSupportedException("This shouldn't be called directly, only inside an expression of a sql builder or helper");
        }
        /// <summary>
        /// Sql min
        ///  It should be used _only_ inside an expression when building a query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static R Min<T,R>(this T table, R expression)
        {
            throw new NotSupportedException("This shouldn't be called directly, only inside an expression of a sql builder or helper");
        }

        /// <summary>
        /// Sql floor
        ///  It should be used _only_ inside an expression when building a query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static R Floor<T,R>(this T table, R expression)
        {
            throw new NotSupportedException("This shouldn't be called directly, only inside an expression of a sql builder or helper");
        }


        /// <summary>
        /// Sql Ceiling
        ///  It should be used _only_ inside an expression when building a query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static R Ceiling<T,R>(this T table, R expression)
        {
            throw new NotSupportedException("This shouldn't be called directly, only inside an expression of a sql builder or helper");
        }

        /// <summary>
        /// Sql avg
        ///  It should be used _only_ inside an expression when building a query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static R Avg<T,R>(this T table, R expression)
        {
            throw new NotSupportedException("This shouldn't be called directly, only inside an expression of a sql builder or helper");
        }


        /// <summary>
        /// Sql round
        ///  It should be used _only_ inside an expression when building a query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static R Round<T, R>(this T table, R expression,int length)
        {
            throw new NotSupportedException("This shouldn't be called directly, only inside an expression of a sql builder or helper");
        }

        /// <summary>
        ///  It should be used _only_ inside an expression when building a query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public static string Concat<T>(this T table, params object[] expressions)
        {
            throw new NotSupportedException("This shouldn't be called directly, only inside an expression of a sql builder or helper");

        }
        
    }
}