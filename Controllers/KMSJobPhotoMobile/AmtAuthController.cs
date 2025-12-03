using AutoMapper;
using entities.Common;
using erpsolution.api.Base;
using erpsolution.dal.Context;
using erpsolution.dal.EF;
using erpsolution.entities;
using erpsolution.entities.Common;
using erpsolution.api.Attribute;
using erpsolution.service.Common.Base.Interface;
using erpsolution.service.Common.Cache;
using erpsolution.service.Interface;
using erpsolution.service.Interface.SystemMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;

namespace erpsolution.api.Controllers.KMSJobPhotoMobile
{
    public class AmtAuthController : ControllerBaseEx<IAmtAuthService, TCMUSMT, decimal>
    {
        private AmtContext _context;
        private IMapper _mapper;
        public IServiceProvider _serviceProvider;
        private readonly ICacheService _memoryCache;
        private readonly IConfiguration _config;
        private readonly IApiExecutionLockService _ApiExcLockService;
        public AmtAuthController(IAmtAuthService service,
       IConfiguration config,
       IServiceProvider serviceProvider,
       AmtContext context,
       ICacheService memoryCache,
       IApiExecutionLockService ApiExcLockService,
       ICurrentUser currentUser) : base(service, currentUser)
        {
            _config = config;
            _memoryCache = memoryCache;
            _serviceProvider = serviceProvider;
            _context = context;
            _ApiExcLockService = ApiExcLockService;
        }

        [ApiExplorerSettings(GroupName = "auth")]
        [HttpPost(nameof(LoginERP))]
        [ProducesResponseType(typeof(HandleResponse<object>), 400)]
        [AllowAnonymous]

        public async Task<IActionResult> LoginERP([FromBody] LoginModel login)
        {
            try
            {
                var user = _service.CheckLoginERP(login);
                if (user != null)
                {
                    return CreateTokenERP(user);
                }
                return Json(new HandleResponse<LoginModel>(false, "Username or password is incorrect", null));
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Auth", login);
                return Json(new HandleResponse<LoginModel>(false, message, null));
            }
        }
        


        private IActionResult CreateTokenERP(TCMUSMT user)
        {

            if (user != null)
            {
                var tokenString = BuildTokenERP(user);
                //string key = "E821752166E916AEEF940855";
                var userInfo = new
                {
                    user_id = user.UserId,
                    full_name = user.Name,
                };
                var encrypedBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userInfo));
                var encrypted = Convert.ToBase64String(encrypedBytes, 0, encrypedBytes.Length);

                _memoryCache.Add<TokenLifeTime>(new TokenLifeTime
                {
                    userName = user.UserId,
                    LoginTime = DateTime.Now,
                    RemoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString()
                }, "TOKEN_LIFETIME_" + user.UserId, 15);
                //lib.WindowsClipboard.SetText($"Bearer {tokenString}");
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        token = tokenString,
                        user_info = encrypted //EncryptUtil.TripleDesEncrypt(JsonConvert.SerializeObject(userInfo),key)
                    }
                });
            }
            //Write token into cache
            return Json(new HandleResponse<LoginModel>(false, "Username or password is incorrect", null));
        }
        private string BuildTokenERP(TCMUSMT masUser)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim("user_name", masUser.Name),
                new Claim("user_id", masUser.UserId),

            };

            var extra = new ClaimsIdentity();
            extra.AddClaim(new Claim(ClaimTypes.Name, masUser.Name));
            extra.AddClaims(claims);

            //var claims = new[] { new Claim(ClaimTypes.Name, masUser.USER_NM) }            

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              _config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              extra.Claims,
              expires: DateTime.Now.AddDays(7),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        

        private async Task<string> LogErrorAsync(Exception ex, string menuName, object vm = null)
        {
            string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            string jsonData = System.Text.Json.JsonSerializer.Serialize(vm);
            var modelAdd = new ApiLogs
            {
                Method = HttpContext.Request.Method,
                ApiName = currentUrl,
                RequestJson = jsonData,
                Message = ex.Message,
                Exception = ex.ToString().Length > 100 ? ex.ToString().Substring(0, 100) : ex.ToString(),
                System = "Mobile",
                MenuName = menuName,
            };
            var log = await _ApiExcLockService.SaveLogError(modelAdd);
            return "Error ID:" + log.LogId + ": " + ex.Message;
        }
    }
}
