using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequestInspector.Function.Models
{
    public class HttpRequestBinHistory
    {
        public HttpRequestBinHistory(RequestBinOptions options)
        {
            Settings = new HttpRequestBinSettings();
            Settings.RequestBinProvider = options.RequestBinProvider;
            Settings.RequestBinRenderer = options.RequestBinRenderer;
            Settings.RequestBinMaxSize = options.RequestBinMaxSize;
            Settings.RequestBodyMaxLength = options.RequestBodyMaxLength;
            Settings.RequestBinSlidingExpiration = options.RequestBinSlidingExpiration;
            Settings.RequestBinAbsoluteExpiration = options.RequestBinAbsoluteExpiration;
        }

        public HttpRequestBinHistory()
        {

        }

        public string BinId;
        public string BinUrl;
        public string ErrorMessage;
        public IList<HttpRequestDescription> RequestHistoryItems;
        public HttpRequestBinSettings Settings;

        public class HttpRequestBinSettings
        {
            public string RequestBinProvider;
            public string RequestBinRenderer;
            public int RequestBinMaxSize;
            public int RequestBodyMaxLength;
            public int RequestBinSlidingExpiration;
            public int RequestBinAbsoluteExpiration;

        }
    }
}
