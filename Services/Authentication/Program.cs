using Application;
using Application.Models;
using Authentication;
using Authentication.Constants;
using Authentication.Providers;
using Authentication.Services;
using Authentication.Validators;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QualityOperationAPI;
using Utils.Helper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Utils.Startup.ConfigConstants(builder, typeof(Authentication.Constants.AUTH));

/* --- Add Repository & Service ---*/




/* --- Set Database ---*/
var connectionString =
    builder.Configuration.GetConnectionString("DBConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<SystemDbContext>(options =>
                    options.UseSqlServer(connectionString));


SetupHttpClient.InitialService(builder);
StartupService.InitialService(builder.Services);
builder.Services.AddMemoryCache();


var EncryptionKeyString = builder.Configuration["Encryption:Base64Key"];
builder.Services.AddSingleton<IEncryptionHelper>(new EncryptionHelper(EncryptionKeyString, isBase64Encoded: false));

builder.Services.AddMapster();



IdentityBuilder identity = builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password settings
    options.Password.RequiredLength = 0;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(AUTH.LOGIN_WAITING_TIME);
    options.Lockout.MaxFailedAccessAttempts = AUTH.MAXIMUM_LOGIN_FAIL;

    // User settings
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddUserManager<ApplicationUserManager>()
    .AddSignInManager<ApplicationSignInManager>()
    .AddPasswordValidator<UserPasswordValidator<ApplicationUser>>()
    .AddTokenProvider<RefreshTokenProvider<ApplicationUser>>("RefreshToken");

builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
{
    o.TokenLifespan = TimeSpan.FromMinutes(AUTH.DATA_PROTECTION_TOKEN_TIMEOUT_MINUTES);
});

AuthenticationBuilder auth = Utils.Startup.ConfigAuthentication(builder);
Utils.Startup.ConfigRequestSize(builder);
Utils.Startup.ConfigCors(builder);
Utils.Startup.ConfigController(builder);
Utils.Startup.ConfigUtils(builder);
Utils.Startup.ConfigSwagger(builder);


Utils.Startup.UseRedis(builder);


builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
    });
#region Custom Identity

bool useMicrosoftAuth = false;
bool.TryParse(builder.Configuration["Authentication:Microsoft:IsEnabled"], out useMicrosoftAuth);
if (useMicrosoftAuth)
{
    auth.AddMicrosoftAccount(options =>
    {
        string tenantId = builder.Configuration["Authentication:Microsoft:TenantId"];
        if (string.IsNullOrEmpty(tenantId) == false)
        {
            options.AuthorizationEndpoint = options.AuthorizationEndpoint.Replace("common", tenantId);
            options.TokenEndpoint = options.TokenEndpoint.Replace("common", tenantId);
        }

        options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];

        options.SaveTokens = true;
    });
}

bool useFacebookAuth = false;
bool.TryParse(builder.Configuration["Authentication:Facebook:IsEnabled"], out useFacebookAuth);
if (useFacebookAuth)
{
    auth.AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    });
}

bool useGoogleAuth = false;
bool.TryParse(builder.Configuration["Authentication:Google:IsEnabled"], out useGoogleAuth);
if (useGoogleAuth)
{
    auth.AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });
}

#endregion

//builder.Services.AddAuthorization(AuthorizePolicy.SetPolicy);
builder.Services.AddAutoMapper(typeof(Program));





var app = builder.Build();

Utils.Startup.UseCors(app);
Utils.Startup.UseSwagger(builder, app);
Utils.Startup.UseForwardedHeaders(app);

// app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
