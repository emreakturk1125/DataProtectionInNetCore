using DataProtection.Web.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DataProtection.Web.Filters
{
    // Metod Seviyesinde Filter tanımladık, Amaç appsetings.json dosyasındaki  WhiteList ve BlackList'e göre request'e cevap dönmek ya da engellemektir.
    public class CheckWhiteListAttribute : ActionFilterAttribute
    {
        private readonly IPList _ipList;

        public CheckWhiteListAttribute(IOptions<IPList> ipList)
        {
            _ipList = ipList.Value;
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var requestIpAddress = context.HttpContext.Connection.RemoteIpAddress;
            var isWhiteList = _ipList.WhiteList.Where(x => IPAddress.Parse(x).Equals(requestIpAddress)).Any();

            if (!isWhiteList) // istek gelen ip adresi appsetting deki whitelist içinde değilse geriye yasaklı kodu gönder demektir.
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                return;
            }

            base.OnActionExecuted(context);
        }
    }
}
