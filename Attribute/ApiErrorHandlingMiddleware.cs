using entities.Common;
using erpsolution.entities.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace erpsolution.api.Attribute
{
    public class ApiErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiErrorHandlingMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<ApiErrorHandlingMiddleware> logger)
        {
            try
            {

                await this._next(context);
                //if (context.Response.StatusCode >= 400)
                //{
                //    await HandleExceptionAsync(context);
                //}
            }

           
            catch (Exception ex)
            {
                string message = string.Format("Message: {0}\n", ex.Message);
                message += string.Format("StackTrace: {0} \n", ex.StackTrace.Replace(Environment.NewLine, string.Empty));
                message += string.Format("Source: {0} \n", ex.Source.Replace(Environment.NewLine, string.Empty));
                message += string.Format("TargetSite: {0}", ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));

                logger.Log(LogLevel.Error, message);
                //await HandleExceptionAsync(context);
                //await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = new { code = context.Response.StatusCode, message = ex.Message } }));
                //context.Response.StatusCode = 400;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new HandleResponse<object>(false, ex.Message,null,null,404) ));
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            if (context.Response.HasStarted)
            {
                //response already written earlier in the pipeline
                return Task.CompletedTask;
            }

            var error = context.Response.StatusCode;
            var msg = string.Empty;
            switch (error) {
                case 404:
                    msg = "API not found";
                    break;
                case 500:
                    msg = "Internal Server Error";
                    break;
                default:
                    msg = "Unknow Server Error";
                    break;
            }            
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = new { code = error,message = msg } }));
        }

    }
}
