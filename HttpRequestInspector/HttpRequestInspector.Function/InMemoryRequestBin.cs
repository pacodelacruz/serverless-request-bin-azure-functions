using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace HttpRequestInspector.Function
{
    public class InMemoryRequestBin : RequestBinBase, IRequestBin
    {
        private const string RequestBinCacheName = "requestBin";
        private static IMemoryCache Cache;
        private static MemoryCacheEntryOptions EntryOptions;

        public InMemoryRequestBin(IMemoryCache cache)
        {
            Cache = cache;
            EntryOptions = new MemoryCacheEntryOptions();
            EntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            EntryOptions.SetAbsoluteExpiration(TimeSpan.FromHours(6));
        }

        public string GetRequest(string id)
        {
            throw new NotImplementedException();
        }

        public void StoreRequest(string id, HttpRequest request)
        {
            var requestDescription = GetRequestDescription(request);
            List<HttpRequestDescription> storedRequests;
            if (Cache.TryGetValue(id, out storedRequests))
            {
                // TODO: Keep the list up to 10 items.
                storedRequests.Add(requestDescription);
            }
            else
                storedRequests = new List<HttpRequestDescription>() { requestDescription };

            Cache.Set(id, storedRequests, EntryOptions);

        }
    }
}
