using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using DotLiquid;
using ServerlessRequestBin.Function.Models;
using ServerlessRequestBin.Function.Extensions;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Resources;

namespace ServerlessRequestBin.Function.Services
{
    /// <summary>
    /// Service to render the Request Bin History into Html for user visualisation
    /// </summary>
    public class LiquidRequestBinRenderer : RequestBinBase, IRequestBinRenderer
    {
        private readonly IRequestBinManager RequestBinManager;
        private readonly Template LiquidTemplate;
        private readonly IOptions<RequestBinOptions> Options;

        public LiquidRequestBinRenderer(IRequestBinManager requestBinManager, IOptions<RequestBinOptions> options) : this(options)
        {
            Options = options;
            RequestBinManager = requestBinManager;
        }

        public LiquidRequestBinRenderer(IOptions<RequestBinOptions> options) : base(options)
        {
            // Read and load the Liquid Template from the embedded resource
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(
                $"{Assembly.GetExecutingAssembly().GetName().Name}.Resources.{options.Value.RequestBinRendererTemplate}")))
            {
                LiquidTemplate = Template.Parse(reader.ReadToEnd());
            }
        }

        public string RenderToString(string binId, string binUrl, string errorMessage = null)
        {
            var requestBinHistory = PrepareBinHistoryForHtml(RequestBinManager.GetRequestBinHistory(binId, binUrl, errorMessage));
            return LiquidTemplate.Render(Hash.FromDictionary(requestBinHistory.ToDictionary()));
        }

        /// <summary>
        /// Encodes string values on the HttpRequestBinHistory to Html so it can be rendered properly
        /// Sorts requests by Timestamp descending
        /// </summary>
        /// <param name="requestBinHistory"></param>
        /// <returns></returns>
        private HttpRequestBinHistory PrepareBinHistoryForHtml(HttpRequestBinHistory requestBinHistory)
        {
            if (requestBinHistory == null)
                return null;
            if (requestBinHistory.RequestHistoryItems == null)
                return requestBinHistory;

            var htmlEncodedRequestBinHistory = new HttpRequestBinHistory();
            htmlEncodedRequestBinHistory.BinId = HttpUtility.HtmlEncode(requestBinHistory.BinId);
            htmlEncodedRequestBinHistory.BinUrl = HttpUtility.HtmlEncode(requestBinHistory.BinUrl);
            htmlEncodedRequestBinHistory.ErrorMessage = HttpUtility.HtmlEncode(requestBinHistory.ErrorMessage);
            htmlEncodedRequestBinHistory.RequestHistoryItems = new List<HttpRequestDescription>();
            foreach (var request in requestBinHistory.RequestHistoryItems.OrderByDescending(i => i.Timestamp).ToList())
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
                    htmlEncodedRequest.QueryParams.Add(new KeyValuePair<string, string>(HttpUtility.HtmlEncode(queryParam.Key), HttpUtility.HtmlEncode(queryParam.Value)));                
                foreach (var header in request.Headers)
                    htmlEncodedRequest.Headers.Add(new KeyValuePair<string, string>(HttpUtility.HtmlEncode(header.Key), HttpUtility.HtmlEncode(header.Value)));
                htmlEncodedRequestBinHistory.RequestHistoryItems.Add(htmlEncodedRequest);
            }
            htmlEncodedRequestBinHistory.Settings = new HttpRequestBinHistory.HttpRequestBinSettings();
            htmlEncodedRequestBinHistory.Settings.RequestBinProvider = requestBinHistory.Settings.RequestBinProvider;
            htmlEncodedRequestBinHistory.Settings.RequestBinRenderer = requestBinHistory.Settings.RequestBinRenderer;
            htmlEncodedRequestBinHistory.Settings.RequestBinMaxSize = requestBinHistory.Settings.RequestBinMaxSize;
            htmlEncodedRequestBinHistory.Settings.RequestBodyMaxLength = requestBinHistory.Settings.RequestBodyMaxLength;
            htmlEncodedRequestBinHistory.Settings.RequestBinSlidingExpiration = requestBinHistory.Settings.RequestBinSlidingExpiration;
            htmlEncodedRequestBinHistory.Settings.RequestBinAbsoluteExpiration = requestBinHistory.Settings.RequestBinAbsoluteExpiration;

            return htmlEncodedRequestBinHistory;
        }
    }
}
