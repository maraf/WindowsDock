using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace WindowsDock.Core
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class ConfigAttribute : Attribute
    {
        public string Name { get; private set; }

        public OnConfigLoad OnLoad { get; private set; }

        public ConfigAttribute(string name)
        {
            Name = name;
        }

        public ConfigAttribute(string name, OnConfigLoad onLoad)
            : this(name)
        {
            OnLoad = onLoad;
        }

        public static IEnumerable<ConfigInfo> GetProperties(Type type)
        {
            foreach (PropertyInfo info in type.GetProperties())
            {
                object[] attrs = info.GetCustomAttributes(typeof(ConfigAttribute), true);
                if (attrs.Length == 1)
                    yield return new ConfigInfo(attrs[0] as ConfigAttribute, info);
            }
        }
    }

    public delegate void OnConfigLoad(string name, object value);

    public class ConfigInfo
    {
        public ConfigAttribute Attribute { get; set; }

        public PropertyInfo Property { get; set; }

        public ConfigInfo(ConfigAttribute attribute, PropertyInfo property)
        {
            Attribute = attribute;
            Property = property;
        }
    }
}
