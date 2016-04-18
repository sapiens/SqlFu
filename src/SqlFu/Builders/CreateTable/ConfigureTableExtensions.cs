using System;
using System.Linq.Expressions;

namespace SqlFu.Builders.CreateTable
{
    public  static class ConfigureTableExtensions
    {
        public static IConfigureTable<T> ColumnDbType<T>(this IConfigureTable<T> table,Expression<Func<T,object>> col, string type) 
            => table.Column(col, c => c.HasDbType(type));

        public static IConfigureTable<T> ColumnSize<T>(this IConfigureTable<T> table,Expression<Func<T,object>> col, int type) 
            => table.Column(col, c => c.HasSize(type));

        public static IConfigureTable<T> ColumnSize<T>(this IConfigureTable<T> table,Expression<Func<T,object>> col, string type) 
            => table.Column(col, c => c.HasSize(type));
        public static IConfigureTable<T> ColumnIsNotNull<T>(this IConfigureTable<T> table,Expression<Func<T,object>> col) 
            => table.Column(col, c => c.NotNull());
        public static IConfigureTable<T> ColumnIsNull<T>(this IConfigureTable<T> table,Expression<Func<T,object>> col) 
            => table.Column(col, c => c.Null());

        public static IConfigureTable<T> DropIfExists<T>(this IConfigureTable<T> table)
            => table.HandleExisting(TableExistsAction.DropIt);
        public static IConfigureTable<T> ThrowIfExists<T>(this IConfigureTable<T> table)
            => table.HandleExisting(TableExistsAction.Throw);
        public static IConfigureTable<T> IgnoreIfExists<T>(this IConfigureTable<T> table)
            => table.HandleExisting(TableExistsAction.Ignore);
    }
}