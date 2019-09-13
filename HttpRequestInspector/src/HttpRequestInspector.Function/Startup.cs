using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using HttpRequestInspector.Function.Services;
using HttpRequestInspector.Function.Models;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(HttpRequestInspector.Function.Startup))]

namespace HttpRequestInspector.Function
{
    /// <summary>
    /// Configures the behaviour of the Functions based on the Environment Variables (App Settings)
    /// Via Constructor Dependency Injection
    /// Default implementations are defined. 
    /// Implements the Options Pattern on Azure Functions here
    /// https://docs.microsoft.com/en-us/azure/architecture/serverless/code
    /// </summary>
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<RequestBinOptions>()
                .Configure<IConfiguration>((configSection, configuration) =>
                    {configuration.Bind(configSection);});

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RequestBinProvider")) ||
                Environment.GetEnvironmentVariable("RequestBinProvider").ToLower() == "memory")
            {
                //InMemoryRequestBinManager is the default implementation
                builder.Services.AddMemoryCache();
                builder.Services.AddSingleton<IRequestBinManager, InMemoryRequestBinManager>();
            }
            else
            {
                throw new NotImplementedException($"RequestBinProvider '{string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RequestBinProvider"))}' not implemented");
            }

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RequestBinRenderer")) ||
                Environment.GetEnvironmentVariable("RequestBinRenderer").ToLower() == "liquid")
            {
                //HtmlRequestBinRenderer is the default implementation
                builder.Services.AddSingleton<IRequestBinRenderer, HtmlRequestBinRenderer>();
            }
            else
            {
                throw new NotImplementedException($"RequestBinRenderer '{string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RequestBinRenderer"))}' not implemented.");
            }
        }
    }
}