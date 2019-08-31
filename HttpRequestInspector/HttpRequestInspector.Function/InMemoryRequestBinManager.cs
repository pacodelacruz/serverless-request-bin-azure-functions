using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace HttpRequestInspector.Function
{
    public class InMemoryRequestBinManager : RequestBinManagerBase, IRequestBinManager
    {
        private const string RequestBinCacheName = "requestBin";
        private static IMemoryCache Cache;
        private static MemoryCacheEntryOptions EntryOptions;

        public InMemoryRequestBinManager(IMemoryCache cache)
        {
            Cache = cache;
            EntryOptions = new MemoryCacheEntryOptions();
            EntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            EntryOptions.SetAbsoluteExpiration(TimeSpan.FromHours(6));
        }

        public string GetRequest(string binId)
        {
            throw new NotImplementedException();
        }

        public void StoreRequest(string binId, HttpRequest request)
        {
            var requestDescription = GetRequestDescription(request);
            List<HttpRequestDescription> storedRequests;
            if (Cache.TryGetValue(binId, out storedRequests))
            {
                // TODO: Keep the list up to 10 items.
                storedRequests.Add(requestDescription);
            }
            else
                storedRequests = new List<HttpRequestDescription>() { requestDescription };

            Cache.Set(binId, storedRequests, EntryOptions);

        }
    }
}
