using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using HttpRequestInspector.Function.Services;

namespace HttpRequestInspector.Function.Extensions
{
    public static class ObjectExtensions
    {
        public static IDictionary<string, object> ToDictionary(this object source)
        {
            // It could be optimised using Reflection
            return JsonConvert.DeserializeObject<IDictionary<string, object>>(JsonConvert.SerializeObject(source), new JsonDictionaryConverter());
        }
    }
}
