using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequestInspector.Function.Models
{
    public class HttpRequestBinHistory
    {
        public string BinId;
        public IList<HttpRequestDescription> RequestHistoryItems;
    }
}
