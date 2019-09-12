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
using HttpRequestInspector.Function.Services;

namespace HttpRequestInspector.Function
{
    public static class Warmer
    {
        [FunctionName("Warmer")]
        public static void WarmUp([TimerTrigger("0 */8 * * * *")]TimerInfo timer)
        {
            //TODO: Log?   
        }
    }
}
