using System;
using System.Linq.Expressions;

namespace SqlFu.Builders.CreateTable
{
    public  static class ConfigureTableExtensions
    {
        public static IConfigureTable<T> ColumnDbType<T>(this IConfigureTable<T> table,Expression<Func<T,object>> col, string type)
        {
            return table.Column(col, c => c.HasDbType(type));
        }

        public static IConfigureTable<T> ColumnSize<T>(this IConfigureTable<T> table,Expression<Func<T,object>> col, int type)
        {
            return table.Column(col, c => c.HasSize(type));
        }
        public static IConfigureTable<T> ColumnSize<T>(this IConfigureTable<T> table,Expression<Func<T,object>> col, string type)
        {
            return table.Column(col, c => c.HasSize(type));
        }
    }
}