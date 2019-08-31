using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace HttpRequestInspector.Function
{
    public class TableStorageRequestBinManager : RequestBinManagerBase, IRequestBinManager
    {
     
        public TableStorageRequestBinManager()
        {

        }

        public string GetRequest(string binId)
        {
            throw new NotImplementedException();
        }

        public void StoreRequest(string binId, HttpRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
