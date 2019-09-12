using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using HttpRequestInspector.Function.Services;

namespace HttpRequestInspector.Function
{
    //TODO: Error Handling
    //TODO: Keep it warm
    //TODO: ARM Template with App Insights and App Settings
    //TODO: Deploy from Git
    //TODO: Add to Serverless Library
    //TODO: Add comments
    //TODO: Add Readme
    //TODO: Test
    public class PersistIntoBin
    {
        private readonly IRequestBinManager RequestBinManager;

        public PersistIntoBin(IRequestBinManager requestBinManager)
        {
            RequestBinManager = requestBinManager;
        }

        [FunctionName("PersistIntoBin")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, 
            "get", "post", "put", "patch", "delete", "head", "options", "trace",
            Route = "{binId}")] HttpRequest request,
            string binId,
            ILogger log)
        {
            try
            {
                log.LogInformation(new EventId(100), "{BinId}, {Message}", binId, $"Request received for bin '{binId}'");
                if (!RequestBinManager.IsBinIdValid(binId, out var validationMessage))
                {
                    log.LogError(new EventId(191), "{BinId}, {Message}", binId, $"Invalid Bin Id '{binId}'.");
                    return new BadRequestObjectResult(validationMessage);
                }
                RequestBinManager.StoreRequest(binId, request);
                log.LogInformation(new EventId(110), "{BinId}, {Message}", binId, $"Request for bin '{binId}' stored.");
                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError(new EventId(190), ex, "{BinId}, {Message}", binId, $"Error occurred while processing request for bin: '{binId}'");
                throw;
            }
        }
    }
}
