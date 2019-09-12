using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using HttpRequestInspector.Function.Models;

namespace HttpRequestInspector.Function.Services
{
    public class InMemoryRequestBinManager : RequestBinBase, IRequestBinManager
    {
        private readonly IMemoryCache Cache;
        private readonly MemoryCacheEntryOptions EntryOptions;

        public InMemoryRequestBinManager(IMemoryCache cache)
        {
            Cache = cache;
            EntryOptions = new MemoryCacheEntryOptions();
            EntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            EntryOptions.SetAbsoluteExpiration(TimeSpan.FromHours(6));
        }

        public HttpRequestBinHistory GetRequestBinHistory(string binId, string binUrl, string errorMessage = null)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                var encodedBinId = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(binId));

                if (Cache.TryGetValue(encodedBinId, out List<HttpRequestDescription> storedRequests))
                    return new HttpRequestBinHistory()
                    {
                        BinId = binId,
                        BinUrl = binUrl,
                        RequestHistoryItems = storedRequests
                    };
                else
                    return new HttpRequestBinHistory()
                    {
                        BinId = binId,
                        BinUrl = binUrl,
                        ErrorMessage = $"Request Bin with Id '{binId}' is empty. Send your requests to {binUrl}.",
                        RequestHistoryItems = null
                    };
            }
            else
                return new HttpRequestBinHistory()
                {
                    BinId = binId,
                    BinUrl = binUrl,
                    ErrorMessage = errorMessage,
                    RequestHistoryItems = null
                };
        }

        public async void StoreRequest(string binId, HttpRequest request)
        {
            var requestDescription = await GetRequestDescription(request);
            List<HttpRequestDescription> storedRequests;

            var encodedBinId = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(binId));

            if (Cache.TryGetValue(encodedBinId, out storedRequests))
            {
                storedRequests.Add(requestDescription);
            }
            else
                storedRequests = new List<HttpRequestDescription>() { requestDescription };

            Cache.Set(encodedBinId, storedRequests, EntryOptions);
        }

        public bool EmptyBin(string binId)
        {
            var encodedBinId = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(binId));
            if (Cache.TryGetValue(encodedBinId, out var storedRequests))
            {
                Cache.Remove(encodedBinId);
                return true;
            }
            else
                return false;
        }
    }
}
