using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SqlFu.Builders.Expressions;

namespace SqlFu
{
    public class InsertSqlOptions:HelperOptions
    {
        private string _identityColumn;

        public string IdentityColumn
        {
            get { return _identityColumn; }
            set
            {
                _identityColumn = value;
                IgnoreColumns.Add(value);
            }
        }

        public List<string> IgnoreColumns { get; }=new List<string>();
    }

    internal class Insertable<T> : InsertSqlOptions,IInsertableOptions<T>
    {
        public void Ignore(params Expression<Func<T, object>>[] columns)
        {
            IgnoreColumns.AddRange(columns.GetNames().ToArray());
        }
    }

    public interface IInsertableOptions<T>:IHelperOptions
    {
        string IdentityColumn { get; set; }

        void Ignore(params Expression<Func<T, object>>[] columns);
    }
}