using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SqlFu.Mapping.Internals
{
    /// <summary>
    /// Not thread safe
    /// </summary>
    public class ConvertersManager : IManageConverters, IRegisterConverter
    {
        public ConvertersManager()
        {
            AddCommonReadConverters();
        }
        /// <summary>
        /// Registers converter from object to specified type. If a converter exists, it replaces it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="converter"></param>
        public void RegisterConverter<T>(Func<object, T> converter)
        {
            Func<object, object> f = o => converter((T)o);
            _converters[typeof (T)] = f;
        }


        public void RegisterWriteConverter<T>(Func<T, object> writeToDb)
        {
            Func<dynamic, object> func = f=> writeToDb(f);
            _writeConverters[typeof(T)] = func;
        }


        Dictionary<Type,Func<object,object>> _converters=new Dictionary<Type, Func<object,object>>();
        
        public bool HasReadConverter(Type type) => _converters.ContainsKey(type);

		readonly List<AWriteConverterRule> _rules = new List<AWriteConverterRule>();

        /// <summary>
        /// Converts value if converters had been specified
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object ProcessBeforeWriting(object value)
		{
			if (value == null) return null;
            //checking for specific type converter
			var act = _writeConverters.GetValueOrDefault(value.GetType());
			if (act != null) return ((Func<dynamic, object>)act)((dynamic)value);
            //checking generic rules
            var rule = _rules.Find(d => d.AppliesTo(value));            
            return rule?.Converter(value)??value;
		}

		Dictionary<Type,object> _writeConverters=new Dictionary<Type, object>();

        /// <summary>
        /// Add/remove rules that will convert values based on type
        /// </summary>
		public List<AWriteConverterRule> Rules => _rules;

		public Func<object, T> GetConverter<T>()
        {
            var c = _converters.GetValueOrDefault(typeof (T));
            if (c == null)
            {
                return DefaultConverter<T>();

            }
            return o=>(T)c(o);
        }

        public static Func<object,T> DefaultConverter<T>() => o =>
        {
            if (o == DBNull.Value)
            {
                if (typeof(T).IsNullable()) return default(T);
                throw new InvalidCastException($"{typeof(T)} is not nullable");
            }
            if (o is T) return (T) o;
                   
            return o.ConvertTo<T>();
        };

        /// <summary>
        /// Adds converters for string, Guid(?), int(?) and byte[]
        /// </summary>
        public void AddCommonReadConverters()
        {
            RegisterConverter(o =>
            {
                if (o == null || o==DBNull.Value) return null;
                return o.ToString();
            });
            RegisterConverter(o=> new InsertedId(o));
            RegisterConverter(o =>
            {
                if (o is Guid guid) return guid;
                if (o is string) return Guid.Parse(o.ToString());
                if (o is byte[]) return new Guid((byte[])o);
                throw new InvalidCastException($"Can't convert null or {o?.GetType()} to Guid");
            }
            );
            
            RegisterConverter(o=> (o==null || o==DBNull.Value)?(Guid?)null:Guid.Parse(o.ToString()));            
            RegisterConverter(o=> (o==null || o==DBNull.Value)?(long?)null:(long)o);            
            RegisterConverter(o=>
            {
                if (o == null || o == DBNull.Value) return (int?) null;
                if (o.GetType() == typeof(long)) return (int) (long) o;
                return (int) o;
            });   
            RegisterConverter(o=>(byte[])o);         
        }

       
        public object ProcessReadValue(Type propertyType,object value)
		{
            var c = _converters.GetValueOrDefault(propertyType, o => o);
            return c(value);
        }
        
        public T Convert<T>(object o) => GetConverter<T>()(o);
    }
}