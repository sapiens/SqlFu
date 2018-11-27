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

        public void RegisterWriteConverter<T>(Func<T, object> writeToDb)
        {
            Func<dynamic, object> func = f=> writeToDb(f);
            _writeConverters[typeof(T)] = func;
        }


        Dictionary<Type,object> _converters=new Dictionary<Type, object>();
        
        public bool HasConverter(Type type) => _converters.ContainsKey(type);

        public object ProcessBeforeWriting(object value)
        {
            if (value == null) return null;
            var act = _writeConverters.GetValueOrDefault(value.GetType());
            if (act == null) return value;
            return ((Func<dynamic, object>) act)((dynamic)value);
        }

        Dictionary<Type,object> _writeConverters=new Dictionary<Type, object>();

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
            RegisterConverter(o =>
            {
                if (o is Guid) return (Guid) o;
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

        public T Convert<T>(object o) => GetConverter<T>()(o);
    }
}