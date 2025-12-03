
using entities.Common;
using erpsolution.entities.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace erpsolution.api.Controllers.Base
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizeExAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public AuthorizeExAttribute(Permission permission)
        {
            this.permission = permission;
        }

        public Permission permission { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    var Claims = context.HttpContext.User.Claims;
                    bool SAVE_YN = bool.Parse(Claims.FirstOrDefault(x => x.Type == "save_yn")?.Value);
                    bool POSTING_YN = bool.Parse(Claims.FirstOrDefault(x => x.Type == "post_yn")?.Value);
                    bool DELETE_YN = bool.Parse(Claims.FirstOrDefault(x => x.Type == "delete_yn")?.Value);
                    bool SEARCH_YN = bool.Parse(Claims.FirstOrDefault(x => x.Type == "search_yn")?.Value);
                    bool PDA_DEVICE_YN = bool.Parse(Claims.FirstOrDefault(x => x.Type == "pda_device_yn")?.Value);

                    switch (permission)
                    {
                        case Permission.SAVE_YN:
                            if (SAVE_YN)
                                return;
                            break;
                        case Permission.POSTING_YN:
                            //if (POSTING_YN)
                                return;
                            //break;
                        case Permission.DELETE_YN:
                            if (DELETE_YN)
                                return;
                            break;
                        case Permission.SEARCH_YN:
                            if (SEARCH_YN)
                                return;
                            break;
                            //case Permission.PDA_DEVICE_YN:
                            //    if()
                    }
                }

                context.Result = new JsonResult(new HandleResponse<object>(false, "Don't has permission", null)); ;
                return;
            }
            catch (Exception)
            {
                
            }
        }
        //public void OnAuthorization(AuthorizationFilterContext context)
        //{
        //    if (context.HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        string user_id = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
        //        string isAdmin = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "is_system_admin")?.Value;
        //        if(isAdmin == "1")
        //        {
        //            //return;
        //        }

        //        IAuthorityService authorityService = context.HttpContext.RequestServices.GetService(typeof(IAuthorityService)) as IAuthorityService;
        //        //ICurrentUser currentUser = context.HttpContext.RequestServices.GetService(typeof(ICurrentUser)) as ICurrentUser;
        //        string RequestedUrl = context.HttpContext.Request.Headers["RequestedUrl"];

        //        //authorize by user only
        //        //var autho =authorityService.CheckPermission(Int32.Parse(user_id ?? "0"), RequestedUrl).Result;
        //        var autho = authorityService.CheckPermission(Int32.Parse(user_id ?? "0")).Result;

        //        if (autho != null)
        //        {
        //            switch (permission)
        //            {
        //                case Permission.SEARCH_YN:
        //                    if(autho.SEARCH_YN)
        //                        return;
        //                    break;
        //                case Permission.SAVE_YN:
        //                    if (autho.SAVE_YN)
        //                        return;
        //                    break;
        //                case Permission.POSTING_YN:
        //                    if (autho.POSTING_YN)
        //                        return;
        //                    break;
        //                case Permission.DELETE_YN:
        //                    if (autho.DELETE_YN)
        //                        return;
        //                    break;
        //            }
        //        }
        //    }
            
        //    context.Result = new JsonResult(new HandleResponse<object>(false, "Don't has permission", null)); ;
        //    return;
        //}
    }
}
