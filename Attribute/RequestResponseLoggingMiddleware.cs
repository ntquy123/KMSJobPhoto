
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace erpsolution.api.Attribute
{
    public class RequestResponseLoggingMiddleware
    {
        const int readChunkBufferLength = 4096;
        private readonly RequestDelegate _next;
 
        private List<string> lsNotWriteLogs = new List<string>() { "/Auth/Login" , "/qrhostpc", "/qrhostpc/negotiate", "/syncdata" , "/auth/CheckPermission" };
        //private readonly ILog _logger = LogManager.GetLogger(typeof(RequestResponseLoggingMiddleware));
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
 
        }
        public async Task Invoke(HttpContext context)
        {
            await LogRequest(context);
            await _next(context);
            //await LogResponse(context);
        }
        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();
            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            if (!lsNotWriteLogs.Contains(context.Request.Path))
            {
                await context.Request.Body.CopyToAsync(requestStream);
            }
            context.Request.Body.Position = 0;
        }
        private static async Task<string> ReadStreamInChunks(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;
            do
            {
                readChunkLength = await reader.ReadBlockAsync(readChunk, 0, readChunkBufferLength);
                await textWriter.WriteAsync(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);
            return textWriter.ToString();
        }

        private async Task LogResponse(HttpContext context)
        {
            //var originalBodyStream = context.Response.Body;
            //await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            //context.Response.Body = responseBody;
            await _next(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            // var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
           
            //await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
