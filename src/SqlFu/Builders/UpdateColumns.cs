using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using SqlFu.Builders.Crud;
using SqlFu.Builders.Expressions;
using SqlFu.Configuration;

namespace SqlFu.Builders
{
    class UpdateColumns : IUpdateColumns
    {
      
       internal  class CreateBuilder<T>: IIgnoreColumns<T>
       {
           private readonly T _data;
           private IEnumerable<string> _ignore;

           public CreateBuilder(T data)
           {
               _data = data;
           }

           public IColumnsToUpdate<T> Ignore(params Expression<Func<T, object>>[] ignore)
           {
               _ignore = ignore.GetNames();
               return this;
           }

           public void PopulateBuilder(UpdateTableBuilder<T> builder)
           {
               _data.ToDictionary().Where(kv=>!_ignore.Contains(kv.Key))
                    .ForEach(kv=>builder.Set(kv.Key,kv.Value));
           }
       }

        public IIgnoreColumns<T> FromData<T>(T data) where T : class
        {
            return new CreateBuilder<T>(data);
        }
    }

    public interface IColumnsToUpdate<T>
    {
        
    }
}