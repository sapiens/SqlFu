using System;
using System.Collections.Generic;

namespace SqlFu.Mapping.Internals
{
    /// <summary>
    /// Not thread safe
    /// </summary>
    public class ConvertersManager : IManageConverters, IRegisterConverter
    {
        public ConvertersManager()
        {
            AddCommonConverters();
        }
        /// <summary>
        /// Registers converter from object to specified type. If a converter exists, it replaces it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="converter"></param>
        public void RegisterConverter<T>(Func<object, T> converter)
        {
            _converters[typeof (T)] = converter;
        }

        public void MapValueObject<T>(Func<T, object> @from, Func<object, T> to = null) 
        {
            _voMap[typeof (T)] = o=>from((T) o);
            if (to!=null) RegisterConverter(to);
        }

        Dictionary<Type,object> _converters=new Dictionary<Type, object>();
        Dictionary<Type,Func<object,object>> _voMap=new Dictionary<Type, Func<object, object>>();

        public bool HasConverter(Type type) => _converters.ContainsKey(type);

        public object ConvertValueObject(object value)
        {
            if (value == null) return null;
            var f = _voMap.GetValueOrDefault(value.GetType());
            f = f ?? (o => o);
            return f(value);
        }

        public Func<object, T> GetConverter<T>()
        {
            var c = _converters.GetValueOrDefault(typeof (T));
            if (c == null)
            {
                return DefaultConverter<T>();

            }
            return (Func<object, T>)c;
        }

        private static Func<object,T> DefaultConverter<T>() => o =>
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
        public void AddCommonConverters()
        {
            RegisterConverter(o =>
            {
                if (o == null || o==DBNull.Value) return null;
                return o.ToString();
            });
            RegisterConverter(o=> new InsertedId(o));
            RegisterConverter(o=> Guid.Parse(o.ToString()));
            //todo test deafult converters            
            //RegisterConverter(o=> (o==null || o==DBNull.Value)?(Guid?)null:Guid.Parse(o.ToString()));            
            RegisterConverter(o=> (o==null || o==DBNull.Value)?(int?)null:(int)o);   
            RegisterConverter(o=>(byte[])o);         
        }

        public T Convert<T>(object o) => GetConverter<T>()(o);
    }
}