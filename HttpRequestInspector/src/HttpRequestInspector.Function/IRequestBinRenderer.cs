using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequestInspector.Function
{
    public interface IRequestBinRenderer
    {
        string RenderToString(string binId);
    }
}
