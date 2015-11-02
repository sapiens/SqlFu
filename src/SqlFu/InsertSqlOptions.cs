using System;
using System.Linq;
using System.Linq.Expressions;
using SqlFu.Configuration;

namespace SqlFu
{
    public class InsertSqlOptions:HelperOptions
    {
        public string IdentityColumn { get; set; }
        public string[] IgnoreColumns { get; set; }=new string[0];
    }

    internal class Insertable<T> : InsertSqlOptions,IInsertableOptions<T>
    {
        public void Ignore(params Expression<Func<T, object>>[] columns)
        {
            IgnoreColumns = columns.GetNames().ToArray();
        }
    }

    public interface IInsertableOptions<T>:IHelperOptions
    {
        string IdentityColumn { get; set; }
        void Ignore(params Expression<Func<T, object>>[] columns);
    }
}