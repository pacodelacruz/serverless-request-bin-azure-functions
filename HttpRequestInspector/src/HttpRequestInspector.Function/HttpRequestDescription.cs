using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequestInspector.Function
{
    public class HttpRequestDescription
    {
        public string Method;
        public string Url;
        public string SourceIp;
        public DateTime Timestamp;
        public List<KeyValuePair<string, string>> Headers;
        public List<KeyValuePair<string, string>> QueryParams;
        public string Body;
    }
}
