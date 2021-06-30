using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "ApiKey";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey)) 
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var configuration = (IConfiguration) context.HttpContext.RequestServices.GetService(typeof(IConfiguration));
            var apiKey = configuration.GetValue<string>("ApiKey");

            if (!apiKey.Equals(potentialApiKey)) 
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}
