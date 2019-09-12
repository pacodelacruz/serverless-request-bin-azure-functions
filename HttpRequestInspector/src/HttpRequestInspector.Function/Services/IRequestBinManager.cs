using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using HttpRequestInspector.Function.Models;

namespace HttpRequestInspector.Function.Services
{
    public interface IRequestBinManager
    {
        void StoreRequest(string binId, HttpRequest request);

        HttpRequestBinHistory GetRequestBinHistory(string binId, string binUrl, string errorMessage);

        bool EmptyBin(string binId);

        bool IsBinIdValid(string binId, out string validationMessage);
    }
}
