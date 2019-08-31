using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequestInspector.Function
{
    public class HttpRequestBinHistory
    {
        public string BinId;
        public IEnumerable<HttpRequestDescription> RequestHistory;
    }
}
