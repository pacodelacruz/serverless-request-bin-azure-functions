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
using ServerlessRequestBin.Function.Services;

namespace ServerlessRequestBin.Function
{
    /// <summary>
    /// Function to Get the request history of a particular Bin. 
    /// The RequestBinManager is defined via Constructor Dependency Injection. 
    /// To call this function submit a request to 
    /// GET http(s)://{{basepath}}/history/{{binId}}
    /// </summary>
    public class GetBin
    {
        private readonly IRequestBinRenderer RequestBinRenderer;

        public GetBin(IRequestBinRenderer requestBinRenderer)
        {
            RequestBinRenderer = requestBinRenderer;
        }

        [FunctionName("GetBin")]
        public async Task<ContentResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, 
            "get", 
            Route = "history/{binId}")] HttpRequest request,
            string binId,
            ILogger log,
            ExecutionContext context)
        {
            try
            {
                log.LogInformation(new EventId(200), "{BinId}, {Message}", binId, $"A request to return request history for bin '{binId}' has been received.");
                if (!RequestBinRenderer.IsBinIdValid(binId, out var validationMessage))
                {
                    log.LogError(new EventId(291), "{BinId}, {Message}", binId, $"Invalid Bin Id '{binId}'.");
                    return NewHtmlContentResult(HttpStatusCode.BadRequest,
                                            RequestBinRenderer.RenderToString(binId, "Invalid", validationMessage));
                }
                var binUrl = $"http{(request.IsHttps ? "s" : "")}://{request.Host}{request.Path.ToString().Replace("/history", "")}";
                var requestBinHistory = RequestBinRenderer.RenderToString(binId, binUrl);
                log.LogInformation(new EventId(210), "{BinId}, {Message}", binId, $"Request history for bin '{binId}' returned successfully.");
                return NewHtmlContentResult(HttpStatusCode.OK, requestBinHistory);
            }
            catch (Exception ex)
            {
                log.LogError(new EventId(290), ex, "{BinId}", binId, $"Error occurred trying to return the request history for bin: '{binId}'");
                try
                {
                    return NewHtmlContentResult(HttpStatusCode.InternalServerError,
                                            RequestBinRenderer.RenderToString(binId, "", $"500 Internal Server Error. Execution Id: '{context.InvocationId.ToString()}'"));
                }
                catch (Exception)
                {
                    //In case the custom Html render didn't work, return a message without format.
                    return NewHtmlContentResult(HttpStatusCode.InternalServerError, 
                                            $"500 Internal Server Error. Execution Id: '{context.InvocationId.ToString()}'");
                }
            }
        }

        private ContentResult NewHtmlContentResult(HttpStatusCode statusCode, string content)
        {
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)statusCode,
                Content = content
            };
        }
    }
}
