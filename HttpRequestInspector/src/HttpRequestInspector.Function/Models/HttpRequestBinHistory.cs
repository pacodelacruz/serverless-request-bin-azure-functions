using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequestInspector.Function.Models
{
    public class HttpRequestBinHistory
    {
        public string BinId;
        public string BinUrl;
        public string ErrorMessage;
        public IList<HttpRequestDescription> RequestHistoryItems;
    }
}
