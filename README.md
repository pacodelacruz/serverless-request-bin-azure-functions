# Serverless Request Bin with Azure Functions

## Quick Deploy to Azure

[![Deploy to Azure](http://azuredeploy.net/deploybutton.svg)](https://azuredeploy.net/)

## Context

If you have developed or consumed HTTP APIs or webhooks, chances are that you have had the need of troubleshooting and inspecting HTTP requests. In the past, there was a very popular and handy free site called Request Bin (requestb.in) that allowed you to capture your HTTP requests and inspect their content, including the body, headers, query params, etc. Unfortunately, due to ongoing abuse, the publicly hosted version of Request Bin was discontinued.

This application allows you to Deploy your own Serverless Request Bin to inspect HTTP Requests in a secure and cost-effective manner.

Consider this a sample solution for personal use. When I was building it, I wanted to try out the new Dependency Injection capabilities of Azure Functions, and the ability to return not only object results but HTML content from functions. I also used a [DotLiquid](https://github.com/dotliquid/dotliquid) template to transform objects to HTML.

The Function App is composed of four functions. Functions are just wrappers that call services. Services are instantiated via Constructor Dependency Injection and configured during the FunctionStartup. The functions are described as follows: 

* **PersistIntoBin**. Persists HTTP requests into a particular bin specified as a path parameter.
* **GetBin**. Gets the HTTP request history for a particular bin specified as a path parameter.
* **EmptyBin**. Deletes the HTTP request history for a particular bin.
* **KeepFunctionAppWarm**. Keeps the Function App warm not only to avoid cold starts but also to keep the in-memory cache. This timer-triggered function does not guarantee against instance recycling or host replacement by the cloud provider.

## Benefits of the Serverless Request Bin

If you deploy your own instance of the Serverless Request Bin, you would get some benefits, including:

* **Owning the Request Bin**, thus having no risk of someone else capturing your sensitive HTTP Requests.
* Having a very **cost-effective **solution, considering the free executions you get and the low cost associated with the corresponding storage.
* **No need of creating a Bin in advance**, the platform will create one if the Bin identifier is not currently in used.
* **Flexible bin identifiers**. You can assign any value you like to the bin identifier, as long as it is not longer than 36 characters, and has no special characters other than hyphen, underscore or dot.
* **Dark Mode** ;)

## How to Deploy your own

Deploying your own instance is very easy. You just need to click on the button at the top, and this will take you to the deployment page. If you are planning to deploy the Serverless Request Bin in a new resource group, it is highly recommended creating the resource group in advance, so you can choose the region for the resource group. At the time of writing, the deploy button option does not allow you to choose the region for a new resource group. Please read the following section to understand the purpose of each of the settings.

## Configuration Options

The configuration options and settings of the Serverless Request Bin are described in the table below. Some of these options are available only at deployment time, while others are also available after deployment as Application Settings of the Function App created. 

| Setting                         | Description                                                                                                                                                                                                                                      | Can be updated after deployment?   |
|---------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------|
| Directory                       | Azure Active Directory Tenant that you want to use to deploy the solution                                                                                                                                                                        | No                                 |
| Subscription                    | Azure subscription in which you want to deploy the solution                                                                                                                                                                                      | No                                 |
| App Name                        | Used to name the different components of the Serverless Request Bin. including the Function App, the consumption plan, Application Insights, and the Azure Storage account.                                                                      | No                                 |
| App Insights Region             | Given that Application Insights is not available in all regions, choose the closest region to the resource group.                                                                                                                                | No                                 |
| Request Bin Provider            | App Setting to configure the Request Bin Provider to store the HTTP request history. Currently, only “Memory” is supported. In the future, other providers might be added. The “Memory” provider keeps the request bin history in a memory cache | Yes                                |
| Request Bin Renderer            | App Setting to configure the Request Bin Renderer to return the Request Bin history to the user. Currently, only “Liquid” is supported. The “Liquid” renderer allows you to convert the Request Bin history object to HTML.                      | Yes                                |
| Request Bin Renderer Template   | File name of the [Liquid template](https://help.shopify.com/en/themes/liquid/basics) to use while rendering the request bin history. Currently, only the “DarkHtmlRender.liquid “ template is provided. You can add your own liquid templates as well.                                               | Yes                                |
| Request Bin Max Size            | Maximum number of request to store in the Request Bin.                                                                                                                                                                                           | Yes                                |
| Request Body Max Length         | Maximum number of characters to read and store of a request body. If a request body is larger than this limit, the body would be truncated.                                                                                                      | Yes                                |
| Request Bin Sliding Expiration  | For the 'Memory' (cache) Provider, the sliding expiration time in minutes. This setting is to configure the In-memory cache, however, the Function App host can be replaced or recycled at any time by the platform.                             | Yes                                |
| Request Bin Absolute Expiration | For the 'Memory' (cache) Provider, the absolute expiration time in minutes. This setting is to configure the In-memory cache, however, the Function App host can be replaced or recycled at any time by the platform.                            | Yes                                |

## How to use it

Using the Serverless Request Bin is very easy. Once you have successfully deployed the Serverless Request Bin, you can use it as follows: 

1. Creating a new Request Bin. Request Bins are created on the fly when the first request to the Request Bin identifier is received. Bin identifiers can be up to 36 characters long and only support digits, letters and the hyphen, underscore and dot symbols.
2. Sending HTTP Requests for inspection.  Send a HTTP request using any of the methods to `https://<yourfunctionappname>.azurewebsites.net/<binId>` e.g. `POST https://<yourfunctionappname>.azurewebsites.net/1234567890?a=1&b=2`
3. Inspecting the Request Bin history. `GET https://<yourfunctionappname>.azurewebsites.net/history/<binId>` e.g. `GET https://<yourfunctionappname>.azurewebsites.net/history/1234567890`
4. Deleting the Request Bin history. `DELETE https://<yourfunctionappname>.azurewebsites.net/history/<binId>` e.g. `DELETE https://<yourfunctionappname>.azurewebsites.net/history/1234567890`

## What you can learn from this solution

You can just use the solution and hopefully, it provides the value you want from it. However, you can also learn some things from the source code, including: 

* **Dependency Injection in Azure Functions**: Constructor Dependency Injection is used to control which service implementations are in charge of the request bin management and rendering. Currently, there is only one implementation for each of the interfaces. However, the solution is prepared to support more implementations.
* **Options Pattern in Azure Functions**. The options pattern is described in detail here and can be used in Azure Functions injecting configuration settings using the `IOptions<T>` interface via Dependency Injection.
* Returning HTML content from an HTTP triggered Azure Function. Most of the HTTP triggered Azure Functions samples we can find on the web return an ObjectResult. However, you can also return a ContentResult, in this case, we are returning content of type text/html.
* Rendering an object into HTML using DotLiquid. You can see how you can transform an object into HTML using [Liquid Templates](https://help.shopify.com/en/themes/liquid/basics) and DotLiquid.

## Limitations of this version of the Serverless Request Bin

This solution should be considered a sample application and only targeted to personal use. There are some known limitations listed below: 

* Ephemerality: On the current version of the Serverless Request Bin, the request history is persisted in a memory cache that can be recycled at any time. There is a timer trigger function that tries to keep the instance warm, however, the hosting instance can be replaced or recycled at any time. 

## Future Work

Being this a sample solution, there is a lot of room for improvement. The most important one being **Durability**: It is on my plans to write a second version of the Serverless Request Bin using [Durable Entities](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-entities). I hope that I can find the time in the near future. Additionally, you might be thinking that it would be much better to have a lightweight Single Page Application that renders in a more elegant way the Request Bin history to the user. I agree, and contributions are more than welcome :)
