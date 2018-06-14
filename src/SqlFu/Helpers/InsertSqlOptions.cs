using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SqlFu.Builders.Expressions;
using SqlFu.Configuration.Internals;

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
                if (!_identityColumn.IsNullOrEmpty())
                {
                    IgnoreProperties.Remove(_identityColumn);
                }
                if (!value.IsNullOrEmpty())
                {
                    IgnoreProperties.Add(value);                    
                }
                _identityColumn = value;
            }
        }

        public InsertSqlOptions(TableInfo info):base(info)
        {
           
            IdentityColumn = info.GetIdentityColumnName();
            info.Columns.Where(d=>d.IgnoreWrite).ForEach(d=>IgnoreProperties.Add(d.Name));
           
        }
    }

    internal class Insertable<T> : InsertSqlOptions,IInsertableOptions<T>
    {
        public Insertable(TableInfo info) : base(info)
        {
        }

        public void Ignore(params Expression<Func<T, object>>[] columns)
        {
            IgnoreProperties.AddRange(columns.GetNames());
        }
    }

    public interface IInsertableOptions<T>:IHelperOptions
    {
        string IdentityColumn { get; set; }

        void Ignore(params Expression<Func<T, object>>[] columns);
    }
}