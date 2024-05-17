using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace AcSys.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNull(this object o)
        {
            return o == null;
        }

        public static bool IsNotNull(this object o)
        {
            return o != null; //!o.IsNull();
        }

        public static bool IsNot<T>(this T o1, T o2) where T : struct
        {
            return !o1.Equals(o2);
        }

        public static T EnsuringThat<T, TException>(this T o, Func<T, bool> assertionAboutO, string message)
            where TException : Exception, new()
        {
            if (assertionAboutO(o))
            {
                return o;
            }

            var e = new TException();
            e.Data.Add("Details", message);
            throw e;
        }

        /// <summary>
        /// 	Responsible for providing a fluent method to apply boolean logic against a reference type.
        /// </summary>
        public static bool Is<T>(this T o, Func<T, bool> a) where T : class
        {
            return a(o);
        }

        /// <summary>
        /// 	Responsible for providing a fluent method to perform a calculation on a reference type.
        /// </summary>
        public static T2 Calculate<T1, T2>(this T1 o, Func<T1, T2> a) where T1 : class
        {
            return a(o);
        }

        /// <summary>
        /// 	Responsible for providing a fluent method to apply boolean logic against a reference type, *and then invert it*.
        /// </summary>
        public static bool IsNot<T>(this T o, Func<T, bool> a) where T : class
        {
            return !a(o);
        }

        /// <summary>
        /// 	Responsible for providing a fluent method to apply boolean logic against a reference type.
        /// </summary>
        public static bool Has<T>(this T o, Func<T, bool> a) where T : class
        {
            return a(o);
        }

        /// <summary>
        /// 	Responsible for providing a fluent method to apply boolean logic against a reference type, *and then invert it*.
        /// </summary>
        public static bool HasNot<T>(this T o, Func<T, bool> a) where T : class
        {
            return !a(o);
        }

        /// <summary>
        /// 	Responsible for providing a fluent method to apply boolean logic against a reference type.
        /// </summary>
        public static bool Does<T>(this T o, Func<T, bool> a) where T : class
        {
            return a(o);
        }

        /// <summary>
        /// 	Responsible for providing a fluent method to apply boolean logic against a reference type, *and then invert it*.
        /// </summary>
        public static bool DoesNot<T>(this T o, Func<T, bool> a) where T : class
        {
            return !a(o);
        }


        #region XML

        public static string SerializeXml(this object obj, Encoding enc, XmlSerializerNamespaces namespaces = null, XmlWriterSettings xmlWriterSettings = null)
        {
            string xml = string.Empty;

            //if (xmlWriterSettings == null)
            //{
            //    xmlWriterSettings = new XmlWriterSettings()
            //    {
            //        // If set to true XmlWriter would close MemoryStream automatically and using would then do double dispose
            //        // Code analysis does not understand that. That's why there is a suppress message.
            //        //CloseOutput = false,
            //        Encoding = enc,

            //        NamespaceHandling = NamespaceHandling.OmitDuplicates,

            //        //OmitXmlDeclaration = true,

            //        Indent = true,

            //        //NewLineChars = Environment.NewLine,
            //        //NewLineHandling = NewLineHandling.Replace,
            //    };
            //}

            using (MemoryStream ms = new MemoryStream())
            {
                XmlWriter xw = null;
                if (xmlWriterSettings == null)
                {
                    xw = XmlWriter.Create(ms);
                }
                else
                {
                    xw = XmlWriter.Create(ms, xmlWriterSettings);
                }

                using (xw)
                {
                    XmlSerializer serializer = new XmlSerializer(obj.GetType());
                    //serializer.Serialize(Console.Out, message);//, namespaces);

                    if (namespaces == null)
                    {
                        serializer.Serialize(xw, obj);
                    }
                    else
                    {
                        serializer.Serialize(xw, obj, namespaces);
                    }
                }

                xml = enc.GetString(ms.ToArray());
            }
            return xml;
        }

        #endregion

        public static string ToJson(this object obj, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (jsonSerializerSettings == null)
            {
                jsonSerializerSettings = new JsonSerializerSettings()
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore, MaxDepth = 1
                };
            }

            string json = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, jsonSerializerSettings);
            return json;
        }
    }
}
