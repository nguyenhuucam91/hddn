using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HDNHD.Core.Helpers
{
    public class ApplicationCache
    {
        private static Dictionary<string, object> _data = new Dictionary<string, object>();

        public static void Save(string key, object obj)
        {
            if (_data.ContainsKey(key))
                _data.Remove(key);
            _data.Add(key, obj);
        }

        public static object Get(string key)
        {
            object value;
            _data.TryGetValue(key, out value);
            return value;
        }

        public static void Clear(string key = null)
        {
            if (key == null)
                _data.Clear();
            else
                _data.Remove(key);
        }
    }
}