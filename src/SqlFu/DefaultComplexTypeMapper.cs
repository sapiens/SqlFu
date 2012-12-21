using System;
using System.Collections.Concurrent;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SqlFu
{
    public class DefaultComplexTypeMapper : IMapComplexType
    {
        public DefaultComplexTypeMapper()
        {
            Separator = '_';
        }

        public bool IsComplex(string value)
        {
            return value.Trim(Separator).IndexOf(Separator) > 0;
        }

        public char Separator { get; set; }

        private static readonly ConcurrentDictionary<Type, Func<dynamic, object>> _creators =
            new ConcurrentDictionary<Type, Func<dynamic, object>>();

        public static void ToCreate<T>(Func<dynamic, object> creator)
        {
            creator.MustNotBeNull();
            _creators.TryAdd(typeof (T), creator);
        }

        internal static Func<dynamic, object> GetCreatorFor(Type tp)
        {
            Func<dynamic, object> rez;
            if (_creators.TryGetValue(tp, out rez))
            {
                return rez;
            }
            return null;
        }

        /// <summary>
        /// returns null if it's not a valid match
        /// </summary>
        /// <param name="poco"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private PropertyInfo[] GetProperties(Type poco, string name)
        {
            var all = name.Split(Separator);
            var currType = poco;
            PropertyInfo p = null;
            var rez = new PropertyInfo[all.Length];
            for (int i = 0; i < all.Length - 1; i++)
            {
                p = currType.GetProperty(all[i]);
                if (p == null) return null;
                if (p.PropertyType.IsValueType) return null;
                rez[i] = p;
                currType = p.PropertyType;
            }
            p = currType.GetProperty(all[all.Length - 1]);
            if (p == null) return null;

            if (Type.GetTypeCode(p.PropertyType) == TypeCode.Object)
            {
                if (!p.PropertyType.IsNullable())
                {
                    var convertibleTypes = new[] {typeof (Guid), typeof (TimeSpan), typeof (CultureInfo)};
                    if (!p.PropertyType.IsEnum && !convertibleTypes.Any(t => t.Equals(p.PropertyType)))
                    {
                        return null;
                    }
                }
                //if(Nullable.GetUnderlyingType(p.PropertyType) == null && p.PropertyType != typeof(Guid)) return null;
            }
            rez[all.Length - 1] = p;
            return rez;
        }

        public virtual void MapType<T>(T poco, IDataReader rd, int idx)
        {
#if !DEBUG
	throw new Exception("Using generated IL");
#endif

            var name = rd.GetName(idx);
            var currType = typeof (T);
            var props = GetProperties(currType, name);
            if (props == null) return;
            object currObj = poco;
            PropertyInfo p;

            for (int i = 0; i < props.Length - 1; i++)
            {
                p = props[i];
                object val = p.GetValueFast(currObj);

                if (val == null)
                {
                    var creator = GetCreatorFor(p.PropertyType);
                    if (creator == null) val = Activator.CreateInstance(p.PropertyType);
                    else
                    {
                        val = creator(poco);
                    }
                    p.SetValueFast(currObj, val);
                }
                currObj = val;
            }

            p = props[props.Length - 1];

            if (rd.IsDBNull(idx))
            {
                if (p.PropertyType.IsValueType && !p.PropertyType.IsNullable())
                {
                    throw new InvalidCastException();
                }
            }

            p.SetValueFast(currObj, rd[idx].ConvertTo(p.PropertyType));
        }

        public virtual void DeclareILVariables(ILGenerator il)
        {
            il.DeclareLocal(typeof (object));
        }


        public virtual bool EmitMapping(ILGenerator il, Type poco, IDataReader rd, int idx)
        {
            var name = rd.GetName(idx);
            var props = GetProperties(poco, name);
            if (props == null) return true;

            PropertyInfo p;

            il.EmitLoadLocal(0);
            il.EmitDup();
            il.EmitStoreLocal(1);


            for (int i = 0; i < props.Length - 1; i++)
            {
                p = props[i];
                var endLoop = il.DefineLabel();


                il.Emit(OpCodes.Callvirt, p.GetGetMethod()); //get property value
                il.Emit(OpCodes.Dup); //value, value

                il.Emit(OpCodes.Ldnull); //value, value, null
                il.Emit(OpCodes.Ceq); //compare to null
                var hasValue = il.DefineLabel();
                il.Emit(OpCodes.Brfalse_S, hasValue); //if not null jump, value stays on stack

                il.Emit(OpCodes.Pop); //remove null

                il.Emit(OpCodes.Ldloc, 1);


                var inv = GetCreatorFor(p.PropertyType);
                if (inv == null)
                {
                    il.Emit(OpCodes.Newobj, p.PropertyType.GetConstructor(Type.EmptyTypes));
                }
                else
                {
                    il.EmitPushType(p.PropertyType);
                    il.Emit(OpCodes.Call,
                            typeof (DefaultComplexTypeMapper).GetMethod("GetCreatorFor",
                                                                        BindingFlags.NonPublic | BindingFlags.Static));

                    il.EmitLoadLocal(0); //load poco
                    il.Emit(OpCodes.Call, typeof (Func<dynamic, object>).GetMethod("Invoke"));
                }

                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Stloc, 1); //curr obj, value

                il.Emit(OpCodes.Callvirt, p.GetSetMethod()); //set val, curr
                il.Emit(OpCodes.Ldloc, 1);
                il.Emit(OpCodes.Br_S, endLoop);
                il.MarkLabel(hasValue);
                il.EmitDup();
                il.EmitStoreLocal(1);

                il.MarkLabel(endLoop);
            }

            p = props[props.Length - 1];

            PocoFactory.EmitGetColumnValue(il, idx, p.PropertyType);

            il.Emit(OpCodes.Callvirt, p.GetSetMethod()); //set final value, stack is empty 

            return true;
        }
    }
}