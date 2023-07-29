using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace LookOn.Core.Helpers
{
    public static class ObjectHelper
    {
        public static List<string> GetPropDescsOrNames<T>() where T : class
        {
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var list = propertyInfos.Select(p =>
                Attribute.IsDefined((MemberInfo) p, typeof(DescriptionAttribute)) ?
                    (Attribute.GetCustomAttribute((MemberInfo) p, typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description :
                    p.Name
            ).ToList();

            return list;
        }
    }
}