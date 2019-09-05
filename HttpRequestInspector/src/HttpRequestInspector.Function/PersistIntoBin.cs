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

                if (string.IsNullOrWhiteSpace(binId) || string.Compare(binId, "bin", true) == 0)
                    return new BadRequestObjectResult("Please pass a bin Id");
                if (binId.Length > 36)
                    return new BadRequestObjectResult("Bin Id cannot be longer than 36 chars");

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
