using erpsolution.entities;
using erpsolution.service.KMSJobPhotoMobile;
using erpsolution.service.Interface;
 
namespace erpsolution.api.Helper
{
    public static partial class RegisterService
    {

        public static IServiceCollection AddRegisterServiceKMSJobPhotoMobile(this IServiceCollection services)
        {

            services.AddScoped<IAmtAuthService, AmtAuthService>();
            services.AddScoped<IAuditResultRegistrationService, AuditResultRegistrationService>();
            //Add Helper
            //   services.AddScoped<IExternalDatabaseQueries, ExternalDatabaseQueries>();


            return services;
        }
    }
}
