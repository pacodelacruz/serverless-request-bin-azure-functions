using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace HttpRequestInspector.Function
{
    public class LiquidRequestBinRenderer : IRequestBinRenderer
    {
        private readonly IRequestBinManager RequestBinManager;

        public LiquidRequestBinRenderer(IRequestBinManager requestBinManager)
        {
            RequestBinManager = requestBinManager;
        }

        public string RenderToString(string binId)
        {
            return "<html><title>123</title></html>";
        }
    }
}
