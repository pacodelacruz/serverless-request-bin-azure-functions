using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequestInspector.Function
{
    public interface IRequestBin
    {
        void StoreRequest(string id, HttpRequest request);

        string GetRequest(string id);
    }
}
