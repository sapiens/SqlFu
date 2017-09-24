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
        public TableInfo Info { get; }
        private string _identityColumn;

        public string IdentityColumn
        {
            get { return _identityColumn; }
            set
            {
                if (!_identityColumn.IsNullOrEmpty())
                {
                    IgnoreColumns.Remove(_identityColumn);
                }
                if (!value.IsNullOrEmpty())
                {
                    IgnoreColumns.Add(value);                    
                }
                _identityColumn = value;
            }
        }

        public List<string> IgnoreColumns { get; }=new List<string>();

        public InsertSqlOptions(TableInfo info):base(info)
        {
           
            IdentityColumn = info.GetIdentityColumnName();
            info.Columns.Where(d=>d.IgnoreWrite).ForEach(d=>IgnoreColumns.Add(d.Name));
           
        }
    }

    internal class Insertable<T> : InsertSqlOptions,IInsertableOptions<T>
    {
        public Insertable(TableInfo info) : base(info)
        {
        }

        public void Ignore(params Expression<Func<T, object>>[] columns)
        {
            IgnoreColumns.AddRange(columns.GetNames());
        }
    }

    public interface IInsertableOptions<T>:IHelperOptions
    {
        string IdentityColumn { get; set; }

        void Ignore(params Expression<Func<T, object>>[] columns);
    }
}