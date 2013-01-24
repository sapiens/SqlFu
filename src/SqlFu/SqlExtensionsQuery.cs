using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using SqlFu.Internals;

namespace SqlFu
{
    public static class SqlExtensionsQuery
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
        public static T Get<T>(this IAccessDb db,object id, string additionalPredicate = null, params object[] args)
        {
            id.MustNotBeNull("id");
            var tp = typeof(T);
            if (typeof(ExpandoObject) == tp || typeof(object) == tp)
                throw new InvalidOperationException("Can't work with System.Object or dynamic types");
            var ti = TableInfo.ForType(tp);
            if (ti.SelectSingleSql == null)
            {
                var sb = new StringBuilder("select ");
                var p = tp.GetProperties().Where(pr => !pr.PropertyType.IsCustomObjectType()).Select(pr => pr.Name);
                foreach (var name in p)
                {
                    sb.AppendFormat("{0},", db.Provider.EscapeName(name));
                }
                sb.Remove(sb.Length - 1, 1);

                sb.AppendFormat(" from {0}", db.Provider.EscapeName(ti.Name));
                sb.AppendFormat(" where {0}=@0", db.Provider.EscapeName(ti.PrimaryKey));
                if (!string.IsNullOrEmpty(additionalPredicate))
                {
                    sb.AppendFormat(" {0}", additionalPredicate);
                }
                ti.SelectSingleSql = sb.ToString();
            }
            var fargs = new List<object>(args.Length + 1);
            fargs.Add(id);
            fargs.AddRange(args);
            return db.Query<T>(ti.SelectSingleSql, fargs.ToArray()).FirstOrDefault();
         
        }

        public static List<T> Fetch<T>(this IAccessDb db,string sql, params object[] args)
        {
            return db.Query<T>(sql, args).ToList();          
        }
    }
}