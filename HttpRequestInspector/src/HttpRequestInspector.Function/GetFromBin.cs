using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace HttpRequestInspector.Function
{
    public class GetFromBin
    {
        private readonly IRequestBinRenderer RequestBinRenderer;

        public GetFromBin(IRequestBinRenderer requestBinRenderer)
        {
            RequestBinRenderer = requestBinRenderer;
        }

        [FunctionName("GetFromBin")]
        public async Task<ContentResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, 
            "get", 
            Route = "bin/{binId}")] HttpRequest request,
            string binId,
            ILogger log)
        {
            try
            {
                log.LogInformation(new EventId(200), "{BinId}, {Message}", binId, $"A request to return request history for bin '{binId}' has been received");

                if (string.IsNullOrWhiteSpace(binId))
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Content = "<html><body>Please provide a valid bin Id</body></html>"
                    };
                if (binId.Length > 36)
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Content = "<html><body>Bin Id cannot be longer than 36 chars</body></html>"
                    };

                var requestBinHistory = RequestBinRenderer.RenderToString(binId);
                log.LogInformation(new EventId(210), "{BinId}, {Message}", binId, $"Request history for bin '{binId}' returned successfully");

                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = "<html><body>Welcome</body></html>"
                };
            }
            catch (Exception ex)
            {
                log.LogError(new EventId(290), ex, "{BinId}", binId, $"Error occurred returning request history for bin: '{binId}'");
                throw;
            }
        }
    }
}
