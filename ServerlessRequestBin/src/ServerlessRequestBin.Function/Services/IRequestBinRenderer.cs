using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessRequestBin.Function.Services
{
    public interface IRequestBinRenderer
    {
        string RenderToString(string binId, string binUrl, string errorMessage = null);

        bool IsBinIdValid(string binId, out string validationMessage);
    }
}
