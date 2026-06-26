using DotNetEnv;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Utils.Extensions;
using Utils.Middleware;

namespace Utils
{
    public partial class Startup
    {
        public static void ConfigConstants(WebApplicationBuilder builder, params Type[] additionals)
        {
            List<Type> constants = new List<Type>()
            {
                typeof(Utils.Constants.COMMON),
                typeof(Utils.Constants.MAIL),
                typeof(Utils.Constants.WEB)
            };
            if (additionals != null)
                constants.AddRange(additionals.ToList());

            foreach (Type constant in constants)
            {
                foreach (PropertyInfo prop in constant.GetProperties())
                {
                    string key = string.Format("Constants:{0}", prop.Name);
                    if (builder.Configuration[key] != null)
                    {
                        if (prop.PropertyType == typeof(int)
                            || prop.PropertyType == typeof(int?))
                        {
                            int i;
                            if (int.TryParse(builder.Configuration[key], out i))
                                prop.SetValue(null, i, null);
                        }
                        else if (prop.PropertyType == typeof(decimal)
                            || prop.PropertyType == typeof(decimal?))
                        {
                            decimal i;
                            if (decimal.TryParse(builder.Configuration[key], out i))
                                prop.SetValue(null, i, null);
                        }
                        else if (prop.PropertyType == typeof(bool)
                                    || prop.PropertyType == typeof(bool?))
                        {
                            bool i;
                            if (bool.TryParse(builder.Configuration[key], out i))
                                prop.SetValue(null, i, null);
                        }
                        else
                            prop.SetValue(null, builder.Configuration[key], null);
                    }
                }
            }
        }
        public static AuthenticationBuilder ConfigAuthentication(WebApplicationBuilder builder)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims

            AuthenticationBuilder auth = builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.RequireAuthenticatedSignIn = true; // Add By Sommai P. Sep 23, 2020
                })
                .AddJwtBearer(cfg =>
                {
                    // cfg.RequireHttpsMetadata = false;
                    // cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["JwtIssuer"],
                        ValidAudience = builder.Configuration["JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"])),

                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(90);
            });
            builder.Services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = true;
            });

            return auth;
        }
        public static void ConfigCors(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                /*
                options.AddPolicy("default", policy =>
                {
                    if (string.IsNullOrWhiteSpace(Utils.Constants.WEB.CLIENT_URL))
                    {
                        policy
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    }
                    else
                    {
                        string[] urls = Utils.Constants.WEB.CLIENT_URL.Split(",").ToArray();

                        policy
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins(urls)
                            .AllowCredentials();
                    }
                });
                */

                //options.AddPolicy("AllowAll",
                //   policy =>
                //   {
                //       policy.AllowAnyOrigin()
                //             .AllowAnyHeader()
                //             .AllowAnyMethod();
                //   });

                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        //policy.WithOrigins("*") // Explicitly allow frontend origin
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                        //.AllowCredentials(); // Use only if credentials (cookies, auth headers) are required
                    });
            });
        }
        public static void UseCors(WebApplication app)
        {
            // app.UseCors("default");

            //app.UseCors("AllowAll");

            app.UseCors("AllowAll");
        }

        public static void ConfigRequestSize(WebApplicationBuilder builder)
        {
            builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });
        }
        public static void ConfigController(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    //options.SerializerSettings.Converters.Add(new Utils.Converters.JsonDateTimeConverter());
                    // UtcDateTimeConverter
                    //options.SerializerSettings.Converters.Add(new Utils.Converters.UtcDateTimeConverter());
                });
        }
        public static void ConfigUtils(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<Web.Services.IFileUpload, Web.Services.FileUpload>();
        }

        public static void ConfigSwagger(WebApplicationBuilder builder)
        {
            bool useSwagger = false;
            if (builder.Configuration["Swagger:Enabled"] != null)
            {
                bool.TryParse(builder.Configuration["Swagger:Enabled"], out useSwagger);
            }
            if (useSwagger)
            {
                string swaggerTitle = "Document";
                if (builder.Configuration["Swagger:Title"] != null)
                {
                    swaggerTitle = builder.Configuration["Swagger:Title"];
                }

                string swaggerVersion = "v1";
                if (builder.Configuration["Swagger:Version"] != null)
                {
                    swaggerVersion = "v" + builder.Configuration["Swagger:Version"];
                }

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(swaggerVersion, new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = swaggerTitle,
                        Version = swaggerVersion
                    });
                    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Name = "Authorization",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                        Description = "Please insert JWT with Bearer info field.",

                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Id = JwtBearerDefaults.AuthenticationScheme,
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
                        }
                    });
                    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                    {
                        {
                            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                            {
                                Scheme = "bearer",
                                BearerFormat = "JWT",
                                Name = "Authorization",
                                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                                Description = "Please insert JWT with Bearer info field.",

                                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                                {
                                    Id = JwtBearerDefaults.AuthenticationScheme,
                                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
                                }
                            },
                            new string[] { }
                        }
                    });
                });
                builder.Services.AddSwaggerGenNewtonsoftSupport();
            }
        }
        public static void UseSwagger(WebApplicationBuilder builder, WebApplication app)
        {
            bool useSwagger = false;
            if (builder.Configuration["Swagger:Enabled"] != null)
            {
                bool.TryParse(builder.Configuration["Swagger:Enabled"], out useSwagger);
            }
            if (useSwagger)
            {
                string swaggerTitle = "Document";
                if (builder.Configuration["Swagger:Title"] != null)
                {
                    swaggerTitle = builder.Configuration["Swagger:Title"];
                }

                string swaggerVersion = "v1";
                if (builder.Configuration["Swagger:Version"] != null)
                {
                    swaggerVersion = "v" + builder.Configuration["Swagger:Version"];
                }



                app.UseSwagger();


                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint(
                        string.Format("/swagger/{0}/swagger.json", swaggerVersion),
                        string.Format("{0} {1}", swaggerTitle, swaggerVersion));
                    c.DefaultModelsExpandDepth(-1); // Ensure this is configured correctly
                    //c.DocExpansion(DocExpansion.None);
                });

            }
        }

        public static void UseForwardedHeaders(WebApplication app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
            });
            app.Use((context, next) =>
            {
                if (Utils.Constants.WEB.SERVER_PROTOCOL != null)
                    context.Request.Scheme = Utils.Constants.WEB.SERVER_PROTOCOL;

                return next();
            });
        }


        public static void UseRedis(WebApplicationBuilder builder)
        {
            var redisConfig = builder.Configuration.GetSection("Redis");
            if (redisConfig.Exists())
            {
                string? host = redisConfig["Host"];
                string? port = redisConfig["Port"];
                string? abortConnect = redisConfig["abortConnect"];
                if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(port) && !string.IsNullOrEmpty(abortConnect))
                {
                    var configOptions = new ConfigurationOptions
                    {
                        EndPoints = { $"{host}:{port}" },
                        AbortOnConnectFail = bool.Parse(abortConnect)
                    };
                    string redisConnectionString = $"{host}:{port},abortConnect={abortConnect}";
                    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                        ConnectionMultiplexer.Connect(configOptions)
                    );
                }
            }

        }


    }

    public partial class StartupAPIMicoService
    {
        public static void StartupCreateBuilder(WebApplicationBuilder builder)
        {
            //Utils.Startup.ConfigConstants(builder, typeof(Authentication.Constants.AUTH));
            builder.Configuration.AddEnvironmentVariables();
            var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
            if (File.Exists(envPath))
            {
                Env.Load(envPath);
                builder.Configuration.AddEnvironmentVariables();
            }

            Utils.Startup.ConfigCors(builder);
            AuthenticationBuilder auth = Utils.Startup.ConfigAuthentication(builder);
            Utils.Startup.ConfigRequestSize(builder);
            Utils.Startup.ConfigController(builder);
            Utils.Startup.ConfigUtils(builder);
            Utils.Startup.ConfigSwagger(builder);
            Utils.Startup.UseRedis(builder);
            builder.Services.AddMemoryCache();
            builder.Services.AddHealthChecks();
            builder.Services.AddHttpContextAccessor();

        }


        public static void StartupCreateApplication(WebApplicationBuilder builder, WebApplication app)
        {

            Utils.Startup.UseCors(app);
            Utils.Startup.UseSwagger(builder, app);
            Utils.Startup.UseForwardedHeaders(app);


            #region Set up Time Zone
            var supportedCultures = new[] { "en" };

            var cultureOptions = ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<IOptions<ApplicationCultureOptions>>();


            var localizationOptions = new RequestLocalizationOptions()
            {
                SupportedUICultures = cultureOptions.Value.GetCultures(supportedCultures),
                SupportedCultures = cultureOptions.Value.GetCultures("en")
            }.SetDefaultCulture("en");
            app.UseRequestLocalization(localizationOptions);

            #endregion
            app.UseMiddleware<CustomErrorHandlerMiddleware>();


            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.MapHealthChecks("/microservice/health/live");

        }
        public static void StartupCreateApplication_CronJob(WebApplicationBuilder builder, WebApplication app)
        {

            Utils.Startup.UseCors(app);
            Utils.Startup.UseSwagger(builder, app);
            Utils.Startup.UseForwardedHeaders(app);


            #region Set up Time Zone
            var supportedCultures = new[] { "en" };

            var cultureOptions = ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<IOptions<ApplicationCultureOptions>>();


            var localizationOptions = new RequestLocalizationOptions()
            {
                SupportedUICultures = cultureOptions.Value.GetCultures(supportedCultures),
                SupportedCultures = cultureOptions.Value.GetCultures("en")
            }.SetDefaultCulture("en");
            app.UseRequestLocalization(localizationOptions);

            #endregion
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }

    }
}