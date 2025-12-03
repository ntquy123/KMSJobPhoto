

using erpsolution.service.Interface.SystemMaster;
using erpsolution.service.SystemMaster;

namespace erpsolution.api.Helper
{
    public static partial class RegisterService
    {

        public static IServiceCollection AddRegisterServiceSYS(this IServiceCollection services)
        {
            services.AddTransient<IApiExecutionLockService, ApiExecutionLockService>();
            return services;
        }
    }
}
