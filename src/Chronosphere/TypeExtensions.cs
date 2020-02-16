using System;
using System.Linq;
using System.Reflection;

namespace Chronosphere
{
    internal static class TypeExtensions
    {
        internal static bool IsSystemClock(this Type type)
        {
            if (type.Name != "ISystemClock") return false;
            if (!type.IsInterface) return false;
            var members = type.GetMembers();
            if (!members.All(x =>
                x is MethodInfo mx && mx.Name == "get_UtcNow" && mx.ReturnType == typeof(DateTimeOffset)
                || x is PropertyInfo px && px.Name == "UtcNow" && px.PropertyType == typeof(DateTimeOffset)
            )) return false;
            return true;
        }
    }
}
