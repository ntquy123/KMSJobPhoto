using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using erpsolution.entities.Common;
using erpsolution.api.Config;

namespace erpsolution.api.Base
{
    [Authorize]
    //[AllowAnonymous]
    //[ApiVersion("v1")]
    public class BaseController : Controller
    {
        /// <summary>
        /// Username that authorized
        /// </summary>
        public string UserCreator => HttpContext?.User.Identity.Name;

        public IdentityUserModel UserIdentity
        {
            get
            {
                var user = HttpContext?.User;
                if (user != null)
                {
                    var claims = user?.Claims.ToList();
                    return new IdentityUserModel
                    {
                        UserId = Convert.ToInt32(claims.FirstOrDefault(x => x.Type == "user_id")?.Value),
                        UserName = user.Identity.Name,
                        FullName = claims.FirstOrDefault(x => x.Type == "full_name")?.Value,
                        CompanyId = Convert.ToInt32(claims.FirstOrDefault(x => x.Type == "company_id")?.Value),
                        //CompanyPackageId = Convert.ToInt32(claims.FirstOrDefault(x => x.Type == "company_pack_id")?.Value),
                        CompanyPackageId = 1,
                        OrgId = Convert.ToInt32(claims.FirstOrDefault(x => x.Type == "org_id")?.Value),
                        WHWorkOrgId = Convert.ToDecimal(claims.FirstOrDefault(x => x.Type == "hr_cd")?.Value),

                    };
                }
                return new IdentityUserModel();

            }
        }

        public bool IsSystemAdmin
        {
            get
            {
                var user = HttpContext.User;
                var claims = user.Claims.ToList();
                return claims.FirstOrDefault(x => x.Type == "is_system_admin")?.Value == "1";
            }
        }

        protected ErrorResponseModel Error(ErrorCode code, string message = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                switch (code)
                {
                    case ErrorCode.DATA_INVALID:
                        message = "Data invalid";
                        break;
                    case ErrorCode.FORBIDDEN:
                        message = "Dont have permission";
                        break;
                    default:
                        message = "System error!";
                        break;
                }
            }
            return new ErrorResponseModel
            {
                Error = new  ErrorModel() { ErrorCode = code.GetHashCode(), ErrorMessage = message }
            };
        }

        public string ClientIp => Request.HttpContext.Connection.RemoteIpAddress.ToString();
        public string ClientInfo => Request.Headers["User-Agent"];

        /// <summary>
        /// Current language request
        /// </summary>
        public string LanguageCol;

        public string RequestedUrl;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RequestedUrl = filterContext.HttpContext.Request.Headers["RequestedUrl"];
            LanguageCol = filterContext.HttpContext.Request.Headers["Language"];
            if (string.IsNullOrEmpty(LanguageCol))
            {
                LanguageCol = "English";
            }
            base.OnActionExecuting(filterContext);
        }

    }
}
