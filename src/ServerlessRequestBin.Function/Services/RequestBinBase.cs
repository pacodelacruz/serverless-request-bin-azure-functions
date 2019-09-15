using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ServerlessRequestBin.Function.Models;
using System.Web;
using System.Linq;
using Microsoft.Extensions.Options;

namespace ServerlessRequestBin.Function.Services
{
    public abstract class RequestBinBase
    {
        private readonly IOptions<RequestBinOptions> Options;

        public RequestBinBase(IOptions<RequestBinOptions> options)
        {
            Options = options;
        }
        protected async Task<HttpRequestDescription> GetRequestDescription(HttpRequest request)
        {
            HttpRequestDescription requestDescription = new HttpRequestDescription();
            requestDescription.Body = await ReadBodyFirstCharsAsync(request, Options.Value.RequestBodyMaxLength);
            //requestDescription.Body = await new StreamReader(request.Body).ReadToEndAsync();
            requestDescription.Method = request.Method;
            requestDescription.SourceIp = request.HttpContext.Connection.RemoteIpAddress.ToString();
            requestDescription.Path = $"{request.Path}{(string.IsNullOrEmpty(request.QueryString.ToString()) ? "" : request.QueryString.ToString())}";
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

        public bool IsBinIdValid(string binId, out string validationMessage)
        {
            validationMessage = "";
            if (string.IsNullOrWhiteSpace(binId))
            {
                validationMessage = "Bin Id cannot be empty.";
                return false;
            }
            else if (binId.Length > 36)
            {
                validationMessage = "Bin Id cannot be longer than 36 chars.";
                return false;
            }
            else if (!binId.All(c => Char.IsLetterOrDigit(c) && (c < 128) || c == '-' || c == '_' || c == '.'))
            {
                validationMessage = "Bin Id can only contain Numbers, Letters, '-', '_' and '.'";
                return false;
            }
            else if (binId.Equals("bin", StringComparison.OrdinalIgnoreCase))
            {
                validationMessage = "Bin Id cannot be 'bin'.";
                return false;
            }

            return true;
        }

        private static async Task<string> ReadBodyFirstCharsAsync(HttpRequest request, int size)
        {
            using (var reader = new StreamReader(request.Body, Encoding.UTF8))
            {
                char[] buffer = new char[size];
                int n = await reader.ReadBlockAsync(buffer, 0, size);
                char[] result = new char[n];
                Array.Copy(buffer, result, n);
                return new string(result);
            }
        }
    }
}
