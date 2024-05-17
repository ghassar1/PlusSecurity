using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace AcSys.Core.Utils
{
    public static class EnumUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">Pass parameter as typeof(enumName)</param>
        /// <returns>Json as string</returns>
        public static string EnumToJson(Type type)
        {
            var data = EnumToDynamicObject(type);
            return JsonConvert.SerializeObject(data);
        }

        public static dynamic EnumToDynamicObject(Type type)
        {
            if (type == null) return null;

            var data = Enum
                .GetNames(type)
                .Select(name => new
                {
                    Id = (int)Enum.Parse(type, name),
                    Name = GetDescription(Enum.Parse(type, name))
                })
                .ToArray();

            return data;
        }

        public static string GetDescription(object enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());
            if (null != fi)
            {
                object[] attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return null;
        }

        public static string[] ToNames<T>() where T : struct
        {
            string[] names = Enum.GetNames(typeof(T));
            return names;
        }

        public static string[] ToDescirpions<T>() where T : struct
        {
            List<string> descriptions = new List<string>();

            Type type = typeof(T);
            string[] names = Enum.GetNames(type);

            foreach (string name in names)
            {
                T t = (T)Enum.Parse(type, name);
                string desc = GetDescription(t);
                descriptions.Add(desc);
            }

            return descriptions.ToArray();
        }

        //public static T ParseDescirpion<T>(string value, bool ignoreCase = false) where T : struct
        //{
        //    T t = null;
        //    string[] descriptions =ToDescirpions<T>();
        //    if(descriptions.Any(d=>d.ToUpper()==value.ToUpper()))
        //    {

        //    }
        //    return t;
        //}
    }
}
