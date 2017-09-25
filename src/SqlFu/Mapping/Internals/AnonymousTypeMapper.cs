using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SqlFu.Mapping.Internals
{
    public class AnonymousTypeMapper<T> : IMapReaderToPoco<T>
    {
        private Func<object[], T> _mapper;

        private static MethodInfo EnumParser = typeof (Enum).GetMethods(BindingFlags.Static | BindingFlags.Public).First(m=>m.Name=="Parse" && m.GetParameters().Length==2);
        public T Map(DbDataReader reader, string parentPrefix = "")
        {
            if (_mapper == null)
            {
                var type = typeof(T); 
                var args = new List<Expression>();
                var input = Expression.Parameter(typeof (object[]));
                var props = type.GetProperties();

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    Expression getValue = Expression.ArrayIndex(input, Expression.Constant(i));
                    if (props[i].PropertyType.IsEnumType() && (reader.GetFieldType(i) == typeof (string)))
                    {
                        getValue = Expression.Call(EnumParser, Expression.Constant(props[i].PropertyType),Expression.Convert(getValue,typeof(string)));
                    }
                    
                    args.Add(Expression.Convert(getValue, props[i].PropertyType));
                }
                
                var newType = Expression.New(type.GetConstructors().First(c => c.GetParameters().Length > 0),args);
                
                _mapper =
                    Expression.Lambda<Func<object[], T>>(newType, input).Compile();
            }
            object[] values=new object[reader.FieldCount];
            reader.GetValues(values);
            return _mapper(values);
        }

        Expression HandleSpecificCase(PropertyInfo pi,Type fieldType)
        {
            
        }
    }
}