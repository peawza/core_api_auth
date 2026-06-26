using Authentication.ApiClients;
using Utils.Extensions;

namespace QualityOperationAPI
{
    public class SetupHttpClient
    {
        public static string BackOfficeAdminApiBase { get; set; }

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public static void InitialService(WebApplicationBuilder builder)
        {


            builder.Services.AddTransient<MicroservicesHandler>();


            builder.Services.AddHttpClient<ISmsApiClients, SmsApiClients>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiClients:SMSApi"]);
                client.Timeout = TimeSpan.FromSeconds(30);
            });





        }
    }
}
