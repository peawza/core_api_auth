using Authentication.Repositories;
using Authentication.Services;
using Microsoft.AspNetCore.Identity;
using Utils.Services;

namespace Authentication
{
    public class LowerInvariantNormalizer : ILookupNormalizer
    {
        public string NormalizeName(string name)
        {
            return name?.ToUpperInvariant();
        }

        public string NormalizeEmail(string email)
        {
            return email?.ToUpperInvariant();
        }
    }


    public class StartupService
    {
        public static void InitialService(IServiceCollection services)
        {
            /* --- Identity --- */
            services.AddScoped<ILookupNormalizer, LowerInvariantNormalizer>();
            /* --- Repositories --- */
            services.AddTransient<IApplicationRepository, ApplicationRepository>();
            services.AddTransient<IEmailService, EmailService>();
            //EmailService : IEmailService

            services.AddTransient<IScreenRepository, ScreenRepository>();
            services.AddTransient<IResourcesRepository, ResourcesRepository>();
            services.AddTransient<ILogImportFileRepository, LogImportFileRepository>();
            services.AddTransient<ICommonRepository, CommonRepository>();


            services.AddTransient<IUMS010Repository, UMS010Repository>();
            services.AddTransient<IUMS020Repository, UMS020Repository>();
            services.AddTransient<IUMS030Repository, UMS030Repository>();

            services.AddTransient<ISSS010Repository, SSS010Repository>();
            services.AddTransient<IOtpRepository, OtpRepository>();

            /* --- Services --- */

            services.AddTransient<IResouresService, ResouresService>();
            services.AddTransient<IScreenService, ScreenService>();
            services.AddTransient<ILogImportFileRepository, LogImportFileRepository>();
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<IUMS010Service, UMS010Service>();
            services.AddTransient<IUMS020Service, UMS020Service>();
            services.AddTransient<IUMS030Service, UMS030Service>();
            services.AddTransient<ISSS010Services, SSS010Services>();



            services.AddTransient<IOtpServic, OtpServic>();
        }
    }
}
