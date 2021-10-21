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
                var max = props.Length > reader.FieldCount ? reader.FieldCount : props.Length;
                for (var i = 0; i < max; i++)
                {
                    Expression getValue = Expression.ArrayIndex(input, Expression.Constant(i));
                    getValue = HandleSpecificCaseIfAny(getValue, props[i].PropertyType, reader.GetFieldType(i));
                   
                    
                    args.Add(Expression.Convert(getValue, props[i].PropertyType));
                }

                
                var newType = Expression.New(type.GetConstructors().First(c => c.GetParameters().Length > 0), args);
                
                _mapper =
                    Expression.Lambda<Func<object[], T>>(newType, input).Compile();
            }
            object[] values=new object[reader.FieldCount];
            reader.GetValues(values);
            return _mapper(values);
        }

        IAnonTypeConverter[] _converters=new IAnonTypeConverter[]{new LongToInt(), new StringToEnum(), };
        Expression HandleSpecificCaseIfAny(Expression getValue,Type dest,Type srcType)
        {
            var converter = _converters.FirstOrDefault(d => d.CanConvert(srcType, dest));
            if (converter == null) return getValue;
            return converter.Convert(srcType, dest, getValue);
        }

        interface IAnonTypeConverter
        {
            bool CanConvert(Type src, Type det);
            Expression Convert(Type src, Type dest, Expression value);
        }

        abstract class AnonConverter:IAnonTypeConverter
        {
            protected readonly Type _src;
            protected readonly Type _dest;

            public AnonConverter(Type src,Type dest)
            {
                _src = src;
                _dest = dest;
            }
            public virtual bool CanConvert(Type src, Type det) => src == _src && det == _dest;
            

            public abstract Expression Convert(Type src, Type dest, Expression value);
        }

        class StringToEnum : AnonConverter
        {
            public StringToEnum() : base(typeof(string), typeof(ValueType))
            {
            }

            public override bool CanConvert(Type src, Type det)
            {
                return src == _src && det.IsEnum;
            }

            public override Expression Convert(Type src, Type dest, Expression value) 
                => Expression.Call(EnumParser, Expression.Constant(dest),
                Expression.Convert(value, typeof(string)));
        }

        class LongToInt:AnonConverter
        {
            public LongToInt() : base(typeof(long), typeof(int))
            {
            }

            public override Expression Convert(Type src, Type dest, Expression value)
            {
               return Expression.Convert(value, typeof(long));
            }
        }
    }

    


}