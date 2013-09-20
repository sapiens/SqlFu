using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SqlFu.Expressions;
using SqlFu.Internals;

namespace SqlFu
{
    public static class QueryHelpers
    {
        /// <summary>
        /// Gets one object, selected by id and an optional predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <param name="additionalPredicate"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T Get<T>(this DbConnection db, object id, string additionalPredicate = null, params object[] args)
          
        {
            id.MustNotBeNull("id");
            var tp = typeof (T);
            if (typeof (ExpandoObject) == tp || typeof (object) == tp)
                throw new InvalidOperationException("Can't work with System.Object or dynamic types");
            var provider = db.GetProvider();
            var ti = TableInfo.ForType(tp);
            if (ti.SelectSingleSql == null)
            {
                var sb = new StringBuilder("select ");
                var p = tp.GetProperties().Where(pr => !pr.PropertyType.IsCustomObjectType()).Select(pr => pr.Name);
                foreach (var name in p)
                {
                    sb.AppendFormat("{0},", provider.EscapeName(name));
                }
                sb.Remove(sb.Length - 1, 1);

                sb.AppendFormat(" from {0}", provider.EscapeName(ti.Name));
                sb.AppendFormat(" where {0}=@0", provider.EscapeName(ti.PrimaryKey));
                if (!string.IsNullOrEmpty(additionalPredicate))
                {
                    sb.AppendFormat(" and {0}", additionalPredicate);
                }
                ti.SelectSingleSql = sb.ToString();
            }
            var fargs = new List<object>(args.Length + 1);
            fargs.Add(id);
            fargs.AddRange(args);
            return db.QuerySingle<T>(ti.SelectSingleSql, fargs.ToArray());
        }

        public static IEnumerable<T> QueryTop<T>(this DbConnection db, int take, string sql, params object[] args)
        {
            var r = db.PagedQuery<T>(0, take, sql, args);
            return r.Items;
        }

        /// <summary>
        /// Selects first row matching criteria and maps it to poco
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static T Get<T>(this DbConnection db, Expression<Func<T, bool>> condition)
        {
            var builder = new ExpressionSqlBuilder<T>(db.GetProvider().BuilderHelper);
            builder
                .WriteSelect()
                .WriteSelectAllColumns()
                .WriteFrom()
                .Where(condition);
            return db.QuerySingle<T>(builder.ToString(), builder.Parameters.ToArray());
        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(this DbConnection db, Expression<Func<T, bool>> condition)
        {
            var builder = new ExpressionSqlBuilder<T>(db.GetProvider().BuilderHelper);
            builder
                .WriteSelect()
                .WriteSelectAllColumns()
                .WriteFrom()
                .Where(condition);
            return db.Query<T>(builder.ToString(), builder.Parameters.ToArray());
        }

        /// <summary>
        /// Returns only the specified column value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="db"></param>
        /// <param name="selector">Column selector</param>
        /// <param name="criteria">Selection criteria</param>
        /// <returns></returns>
        public static R GetColumnValue<T, R>(this DbConnection db, Expression<Func<T, R>> selector,
                                             Expression<Func<T, bool>> criteria)
        {
            selector.MustNotBeNull();
            var builder = new ExpressionSqlBuilder<T>(db.GetProvider().BuilderHelper);
            builder
                .WriteSelect()
                .WriteSelectColumn(selector)
                .WriteFrom()
                .Where(criteria);

            return db.GetValue<R>(builder.ToString(), builder.Parameters.ToArray());
        }

        public static IEnumerable<R> QueryColumn<T, R>(this DbConnection db, Expression<Func<T, R>> selector,
                                                       Expression<Func<T, bool>> criteria)
        {
            selector.MustNotBeNull();
            var builder = new ExpressionSqlBuilder<T>(db.GetProvider().BuilderHelper);
            builder
                .WriteSelect()
                .WriteSelectColumn(selector)
                .WriteFrom()
                .Where(criteria);

            return db.Query<R>(builder.ToString(), builder.Parameters.ToArray());
        }

        public static long Count<T>(this DbConnection db, Expression<Func<T, bool>> criteria = null)
        {
            var builder = new ExpressionSqlBuilder<T>(db.GetProvider().BuilderHelper);
            builder.Append("select count(*) from ").WriteTableName();
            if (criteria != null)
            {
                builder.Where(criteria);
            }
            return db.GetValue<long>(builder.ToString(), builder.Parameters.ToArray());
        }

        /// <summary>
        /// Checks if the table has any rows with optional criteria
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static bool HasAnyRows<T>(this DbConnection db, Expression<Func<T, bool>> criteria = null)
        {
            var builder = new ExpressionSqlBuilder<T>(db.GetProvider().BuilderHelper);
            builder
                .Append("select 1 from ")
                .WriteTableName()
                .Append(" where exists(select * from ")
                .WriteTableName();
            if (criteria != null)
            {
                builder.Where(criteria);
            }
            builder.Append(")");
            var r = db.GetValue<int?>(builder.ToString(), builder.Parameters.ToArray());
            if (r.HasValue) return true;
            return false;
        }
    }
}