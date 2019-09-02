using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequestInspector.Function
{
    public interface IRequestBinManager
    {
        void StoreRequest(string binId, HttpRequest request);

        HttpRequestBinHistory GetRequestBinHistory(string binId);
    }
}
