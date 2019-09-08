using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using DotLiquid;
using HttpRequestInspector.Function.Models;
using System.IO;

namespace HttpRequestInspector.Function.Services
{
    public class HtmlRequestBinRenderer : IRequestBinRenderer
    {
        private readonly IRequestBinManager RequestBinManager;
        private readonly Template LiquidTemplate;

        public HtmlRequestBinRenderer(IRequestBinManager requestBinManager) : base()
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
            var requestBinHistoryAsHash = Hash.FromDictionary(JsonConvert.DeserializeObject<IDictionary<string, object>>(JsonConvert.SerializeObject(requestBinHistory)));
            //var requestBinHistoryAsHash = 

            //var json = JsonConvert.DeserializeObject<IDictionary<string, object>>(@"{ ""names"":[{""name"": ""John""},{""name"":""Doe""}]  }", new DictionaryConverter());
            var jsonHash = Hash.FromDictionary(json);
            var templatetest = "<h1>{{device}}</h1><h2>{{speed}}</h2>{% for client in names %}<h4>{{client.name}}</h4>{% endfor %}";

            var template = Template.Parse(templatetest);
            var render = template.Render(jsonHash);

            if (requestBinHistory != null)
            {
                var renderedHtml = LiquidTemplate.Render(Hash.FromAnonymousObject(new { RequestBinHistory = requestBinHistory }));
                return renderedHtml;
            }
            else
                return "";           
        }

        /// <summary>
        /// Encodes string values on the HttpRequestBinHistory to Html so it can be rendered properly
        /// </summary>
        /// <param name="requestBinHistory"></param>
        /// <returns></returns>
        private HttpRequestBinHistory HtmlEncodeProperties(HttpRequestBinHistory requestBinHistory)
        {
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
