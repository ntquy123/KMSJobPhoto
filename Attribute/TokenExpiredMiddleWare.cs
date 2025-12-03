using entities.Common;
using erpsolution.entities.Common;
using erpsolution.entities.Setting;
using erpsolution.service.Common.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace erpsolution.api.Attribute
{
    public class TokenExpiredMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ICacheService _memoryCache;
        private readonly string MEMORY_KEY = "TOKEN_LIFETIME_";
        private readonly IOptions<Jwt> _options;
        public TokenExpiredMiddleWare(RequestDelegate next, ICacheService memoryCache, IOptions<Jwt> options)
        {
            _options = options;
            _next = next;
            _memoryCache = memoryCache;
        }
        public async Task Invoke(HttpContext context) {
            await Check(context);
            await _next(context);
        }
        private async Task Check(HttpContext context)
        {
            //TokenExpired
            if (!_options.Value.DisabledExpire && context.User.Identity.IsAuthenticated && !context.Request.Path.Value.Contains("Login"))
            {
                string userName= context.User.Identity.Name;
                string TOKEN_KEY = MEMORY_KEY + userName;
                var Claims = context.Request.HttpContext.User.Claims;
                bool PDA_DEVICE_YN = bool.Parse(Claims.FirstOrDefault(x => x.Type == "pda_device_yn")?.Value);
                if (PDA_DEVICE_YN)
                    return;
                TokenLifeTime tokenLifeTime = _memoryCache.Get<TokenLifeTime>(TOKEN_KEY);
                if(tokenLifeTime == null )
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new HandleResponse<object>(false, "TOKEN_EXPIRED", null, null, 401)));
                }else if(!_options.Value.AllowMultiDevice)
                {
                    string ip = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
                    if(ip != tokenLifeTime.RemoteIpAddress)
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new HandleResponse<object>(false, "ACCOUNT_HAS_USING_FROM_ANOTHER_DEVICE", null, null, 498)));
                    }
                }
                else
                {
                    _memoryCache.Add<TokenLifeTime>(tokenLifeTime, TOKEN_KEY, _options.Value.TimeLife ?? 15) ;
                }
            }
            
           

        }
    }
    
}
