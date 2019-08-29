using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequestInspector.Function
{
    public abstract class RequestBinBase
    {
        protected HttpRequestDescription GetRequestDescription(HttpRequest request)
        {
            HttpRequestDescription requestDescription = new HttpRequestDescription();
            requestDescription.Method = request.Method;
            requestDescription.SourceIp = request.HttpContext.Connection.RemoteIpAddress.ToString();
            requestDescription.Url = request.Path;
            requestDescription.QueryParams = new List<KeyValuePair<string, string>>();
            requestDescription.Headers = new List<KeyValuePair<string, string>>();

            foreach (var param in request.Query)
            {
                requestDescription.QueryParams.Add(new KeyValuePair<string, string>(param.Key, param.Value));
            }

            foreach (var header in request.Headers)
            {
                requestDescription.Headers.Add(new KeyValuePair<string, string>(header.Key, header.Value));
            }

            return requestDescription;
        }
    }
}
