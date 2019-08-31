using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HttpRequestInspector.Function
{
    public class Listener
    {
        private static IRequestBinManager RequestBin;

        public Listener(IRequestBinManager requestBin)
        {
            RequestBin = requestBin;
        }

        [FunctionName("Listener")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, 
            "get", "post", "put", "patch", "delete", "head", "options", "trace",
            Route = "bin/{id}")] HttpRequest request,
            string id,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                if (string.IsNullOrWhiteSpace(id))
                    return new BadRequestObjectResult("Please pass an id for the bin");

                RequestBin.StoreRequest(id, request);

                string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);


                return id != null
                    ? (ActionResult)new OkObjectResult($"Hello, {id}")
                    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
