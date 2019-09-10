using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using DotLiquid;
using HttpRequestInspector.Function.Models;
using HttpRequestInspector.Function.Extensions;
using System.IO;

namespace HttpRequestInspector.Function.Services
{
    public class HtmlRequestBinRenderer : IRequestBinRenderer
    {
        private readonly IRequestBinManager RequestBinManager;
        private readonly Template LiquidTemplate;
        
        public HtmlRequestBinRenderer(IRequestBinManager requestBinManager) : this()
        {
            RequestBinManager = requestBinManager;
        }

        public HtmlRequestBinRenderer()
        {
            LiquidTemplate = Template.Parse(File.ReadAllText(@"Templates\HtmlRender.liquid", Encoding.UTF8));            
        }

        public string RenderToString(string binId)
        {
            var requestBinHistory = HtmlEncodeProperties(RequestBinManager.GetRequestBinHistory(binId));

            if (requestBinHistory != null)
            {
                //var dict = requestBinHistory.ToDictionary();
                //var hash = Hash.FromDictionary(dict);
                var renderedHtml = LiquidTemplate.Render(Hash.FromDictionary(requestBinHistory.ToDictionary()));
                return renderedHtml;
            }
            else
                //TODO: Return Message of NOT Found
                return "";           
        }

        /// <summary>
        /// Encodes string values on the HttpRequestBinHistory to Html so it can be rendered properly
        /// </summary>
        /// <param name="requestBinHistory"></param>
        /// <returns></returns>
        private HttpRequestBinHistory HtmlEncodeProperties(HttpRequestBinHistory requestBinHistory)
        {
            if (requestBinHistory == null)
                return null;

            var htmlEncodedRequestBinHistory = new HttpRequestBinHistory();
            htmlEncodedRequestBinHistory.BinId = HttpUtility.HtmlEncode(requestBinHistory.BinId);
            htmlEncodedRequestBinHistory.RequestHistoryItems = new List<HttpRequestDescription>();
            foreach (var request in requestBinHistory.RequestHistoryItems)
            {
                var htmlEncodedRequest = new HttpRequestDescription();
                htmlEncodedRequest.Body = HttpUtility.HtmlEncode(request.Body);
                htmlEncodedRequest.Method = HttpUtility.HtmlEncode(request.Method);
                htmlEncodedRequest.Path = HttpUtility.HtmlEncode(request.Path);
                htmlEncodedRequest.SourceIp = HttpUtility.HtmlEncode(request.SourceIp);
                htmlEncodedRequest.Timestamp = request.Timestamp;
                htmlEncodedRequest.QueryParams = new List<KeyValuePair<string, string>>();
                htmlEncodedRequest.Headers = new List<KeyValuePair<string, string>>();
                foreach (var queryParam in request.QueryParams)
                {
                    htmlEncodedRequest.QueryParams.Add(new KeyValuePair<string, string>(HttpUtility.HtmlEncode(queryParam.Key), HttpUtility.HtmlEncode(queryParam.Value)));
                }
                foreach (var header in request.Headers)
                {
                    htmlEncodedRequest.Headers.Add(new KeyValuePair<string, string>(HttpUtility.HtmlEncode(header.Key), HttpUtility.HtmlEncode(header.Value)));
                }
                htmlEncodedRequestBinHistory.RequestHistoryItems.Add(htmlEncodedRequest);
            }
            return htmlEncodedRequestBinHistory;
        }
    }
}
