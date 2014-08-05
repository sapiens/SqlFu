using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;


namespace SqlFu
{
    public static class Utils
    {
        public static string FormatCommand(this IDbCommand cmd)
        {
            return FormatCommand(cmd.CommandText,
                                 (cmd.Parameters.Cast<IDbDataParameter>()
                                     .ToDictionary(p => p.ParameterName, p => p.Value)));
        }

        public static bool IsListParam(this object data)
        {
            if (data == null) return false;
            //var type = data.GetType();
            return data is IEnumerable && !(data is string) && !(data is byte[]);
           // return type.Implements<IEnumerable>() && typeof (string) != type && typeof (byte[]) != type;
        }

        public static string FormatCommand(string sql, IDictionary<string, object> args)
        {
            var sb = new StringBuilder();
            if (sql == null)
                return "";
            sb.Append(sql);
            if (args != null && args.Count > 0)
            {
                sb.Append("\n");
                foreach (var kv in args)
                {
                    sb.AppendFormat("\t -> {0} [{1}] = \"{2}\"\n", kv.Key, kv.Value.GetType().Name, kv.Value);
                }

                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        public static bool IsCustomObjectType(this Type t)
        {
            return t.IsClass && (Type.GetTypeCode(t) == TypeCode.Object);
        }
        
        public static bool IsCustomObject<T>(this T t)
        {
            return !(t is ValueType) && (Type.GetTypeCode(t.GetType()) == TypeCode.Object);
        }

        public static T[] GetModelAttributes<T>(this ICustomAttributeProvider self) where T : Attribute
        {
            // Try to get the attribute from self first.
            var result = self.GetCustomAttributes<T>();
            
            if (result.Length==0)
            {
                // Attribute not found on self, so look for it in the metadata buddy class.
                if (self is PropertyInfo)
                {
                    // Looking up a property attribute.
                    var propInfo = (PropertyInfo)self;
                    var propIndexParams = propInfo.GetIndexParameters();
                    List<Type> propIndexTypes = new List<Type>();
                    if (propIndexParams != null)
                    {
                        foreach (var p in propIndexParams)
                        {
                            propIndexTypes.Add(p.ParameterType);
                        }
                    }
                  //  var metaTypeAttr = propInfo.DeclaringType.GetSingleAttribute<MetadataTypeAttribute>();
                    var metaTypeAttr = propInfo.ReflectedType.GetSingleAttribute<MetadataTypeAttribute>();
                    if (metaTypeAttr != null)
                    {
                        var metaProp = metaTypeAttr.MetadataClassType.GetProperty(propInfo.Name, propInfo.PropertyType, propIndexTypes.ToArray());
                        if (metaProp != null)
                            result = metaProp.GetCustomAttributes<T>().ToArray();
                    }
                }
                else
                {
                    // Looking up a class attribute.
                    var metaTypeAttr = self.GetSingleAttribute<MetadataTypeAttribute>();
                    if (metaTypeAttr != null)
                        result = metaTypeAttr.MetadataClassType.GetCustomAttributes<T>().ToArray();
                }
            }
            return result;
        }

       
        /// <summary>
        /// Enhances the <see cref="GetSingleAttribute<T>"/> extension introduced in CavemanTools assembly by 
        /// adding the alsoCheckMetadataType parameter that controls if a metadata buddy class will be searched
        /// for the attribute of type T in case it's not defined in the original POCO class. The idea here is
        /// to keep POCO's SqlFu agnostic by moving SqlFu attributes to a buddy class.
        /// </summary>
        public static T GetSingleAttribute<T>(this ICustomAttributeProvider self, bool alsoCheckMetadataType) where T : Attribute
        {
            if (alsoCheckMetadataType) return GetModelAttributes<T>(self).FirstOrDefault();
            return self.GetSingleAttribute<T>();
            //// Try to get the attribute from self first.
            //var result = self.GetSingleAttribute<T>();

            //if (result == null && alsoCheckMetadataType)
            //{
            //    // Attribute not found on self, so look for it in the metadata buddy class.
            //    if (self is PropertyInfo)
            //    {
            //        // Looking up a property attribute.
            //        var propInfo = (PropertyInfo)self;
            //        var propIndexParams = propInfo.GetIndexParameters();
            //        List<Type> propIndexTypes = new List<Type>();
            //        if (propIndexParams != null)
            //        {
            //            foreach (var p in propIndexParams)
            //            {
            //                propIndexTypes.Add(p.ParameterType);
            //            }
            //        }
            //        var metaTypeAttr = propInfo.DeclaringType.GetSingleAttribute<MetadataTypeAttribute>();
            //        if (metaTypeAttr != null)
            //        {
            //            var metaProp = metaTypeAttr.MetadataClassType.GetProperty(propInfo.Name, propInfo.PropertyType, propIndexTypes.ToArray());
            //            if (metaProp != null)
            //                result = metaProp.GetSingleAttribute<T>();
            //        }
            //    }
            //    else
            //    {
            //        // Looking up a class attribute.
            //        var metaTypeAttr = self.GetSingleAttribute<MetadataTypeAttribute>();
            //        if (metaTypeAttr != null)
            //            result = metaTypeAttr.MetadataClassType.GetSingleAttribute<T>();
            //    }
            //}
            //return result;
        }
    }
}
