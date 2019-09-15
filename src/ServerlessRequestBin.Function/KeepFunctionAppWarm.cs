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
    public static class KeepFunctionAppWarm
    {
        /// <summary>
        /// Keeps the serverless funcion app warm not only to avoid cold starts, but also to keep the in-memory cache. 
        /// This timer tiggered function does not guarantee against instance recycling or switching by the cloud provider. 
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="log"></param>
        /// <param name="context"></param>
        [FunctionName("KeepFunctionAppWarm")]
        public static void Run(
            [TimerTrigger("0 */5 * * * *")] TimerInfo timer,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation(new EventId(901), $"Keeping function app warm using the {context.FunctionName} function");
        }
    }
}
