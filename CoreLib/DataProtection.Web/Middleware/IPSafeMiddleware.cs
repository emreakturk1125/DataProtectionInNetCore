using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DataProtection.Web.Middleware
{
    // Uygulama Seviyesinde Middleware tanımladık, startup.cs de ayarlamalar yaptık. Amaç appsetings.json dosyasındaki  WhiteList ve BlackList'e göre request'e cevap dönmek ya da engellemektir.
    public class IPSafeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IPList _ipList;

        public IPSafeMiddleware(RequestDelegate next,IOptions<IPList> ipList)
        {
            _next = next;
            _ipList = ipList.Value;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var requestIpAddress = httpContext.Connection.RemoteIpAddress;
            var isWhiteList = _ipList.WhiteList.Where(x => IPAddress.Parse(x).Equals(requestIpAddress)).Any();
            if (!isWhiteList) // istek gelen ip adresi appsetting deki whitelist içinde değilse geriye yasaklı kodu gönder demektir.
            {
                httpContext.Response.StatusCode = (int)StatusCodes.Status403Forbidden;
                return;
            }

            await _next(httpContext);  //  istek gelen ip adresi appsetting deki whitelist içinde ise devam et  demektir. Gelen isteği bir sonraki middleware e yönlendirir
        }

    }
}
