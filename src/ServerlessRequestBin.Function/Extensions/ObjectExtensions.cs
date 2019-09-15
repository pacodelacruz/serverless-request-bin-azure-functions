using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using ServerlessRequestBin.Function.Services;

namespace ServerlessRequestBin.Function.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Object extension to desieralise an object into a Dictionary of string and objects throughout the object hierarchy
        /// It could be optimised using reflection
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IDictionary<string, object> ToDictionary(this object source)
        {
            return JsonConvert.DeserializeObject<IDictionary<string, object>>(JsonConvert.SerializeObject(source), new JsonDictionaryConverter());
        }
    }
}
