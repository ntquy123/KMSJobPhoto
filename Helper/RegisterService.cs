using erpsolution.service.Common.Base;
using erpsolution.service.Common.Base.Interface;
using erpsolution.service.Common.Cache;
using System.Security.Claims;
using System.Security.Principal;
namespace erpsolution.api.Helper
{
    public static partial class RegisterService
    {

        public static IServiceCollection AddRegisterService(this IServiceCollection services)
        {
            //ms sql db context

           // services.AddTransient<IAuthorityService, AuthorityService>();
            //services.AddTransient<IPkTbMasCodeNameService, PkTbMasCodeNameService>();

            //cache
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IPrincipal, ClaimsPrincipal>(provider => provider.GetService<IHttpContextAccessor>()?.HttpContext?.User);
            services.AddTransient<ICurrentUser, CurrentUser>();

            //services.AddSingleton<IStringLocalizerFactory, EFStringLocalizerFactory>();
            //services.AddSingleton<IStringLocalizer, EFStringLocalizer>();


            services.AddRegisterServiceSYS()
                 .AddRegisterServiceKMSJobPhotoMobile();

            return services;
        }

    }
}
