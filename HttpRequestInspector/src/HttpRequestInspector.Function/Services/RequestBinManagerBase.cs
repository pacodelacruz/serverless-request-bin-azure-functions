using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HttpRequestInspector.Function.Models;
using System.Web;

namespace HttpRequestInspector.Function.Services
{
    public abstract class RequestBinManagerBase
    {
        protected async Task<HttpRequestDescription> GetRequestDescription(HttpRequest request)
        {
            HttpRequestDescription requestDescription = new HttpRequestDescription();
            
            requestDescription.Body = await new StreamReader(request.Body).ReadToEndAsync();
            requestDescription.Method = request.Method;
            requestDescription.SourceIp = request.HttpContext.Connection.RemoteIpAddress.ToString();
            requestDescription.Path = request.Path;
            requestDescription.Timestamp = DateTime.UtcNow;
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
