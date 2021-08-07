using System;
using System.Collections.Generic;

namespace Internals
{
    public static class Locator
    {
        private static Dictionary<Type, object> _map = new Dictionary<Type, object>();

        public static void Register(Type type, object @object) => _map[type] = @object;
        public static void Unregister(Type type) => _map.Remove(type);

        public static T GetObject<T>() where T : class
        {
            if (!_map.ContainsKey(typeof(T))) return null;
            else return (T)_map[typeof(T)];
        }
    }
}