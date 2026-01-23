using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Authorization;
using erpsolution.api.Attribute;
using Microsoft.AspNetCore.Mvc;
using entities.Setting;
using erpsolution.entities.Setting;
using System.Reflection;
using Microsoft.OpenApi.Models;
using erpsolution.api.Helper;
using Microsoft.Extensions.FileProviders;
using dal.Context.Setting;
using erpsolution.dal.Context;
using Microsoft.EntityFrameworkCore;
using erpsolution.dal.EF;
using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNetCore.StaticFiles;


namespace erpsolution.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            // Rebuild the configuration so environment specific appsettings files are loaded consistently.
            Configuration = new ConfigurationBuilder()
                .SetBasePath(hostEnvironment.ContentRootPath)
                .AddConfiguration(configuration)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomDbContext(Configuration);
            /* Authentication */
            //services.AddCors(config =>
            //{
            //    var policy = new CorsPolicy();
            //    policy.Headers.Add("*");
            //    policy.Methods.Add("*");
            //    policy.Origins.Add("*");
            //    policy.SupportsCredentials = true;
            //    config.AddPolicy("policy", policy);
            //});

            services.AddCors();
            services.AddControllers(options =>
            {
                //Handle Model state
                options.Filters.Add(typeof(ModelStateValidationFilter));
                //Group multi Defination on swagger
                options.Conventions.Add(new ApiExplorerGroupPerVersionConvention());


            }).AddNewtonsoftJson(options =>
            {
                // Use the default property (Pascal) casing
                //options.SerializerSettings.ContractResolver = new DefaultContractResolver();

                // Configure a custom converter
                //options.SerializerSettings.ContractResolver = new JsonLowercaseContractResolver();
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                //For display Enum in swagger
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.Converters.Add(new DecimalConverter());

            });
            services.AddSwaggerGenNewtonsoftSupport(); //
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;

                        var lstHub = new List<string> { "/qrhostpc", "/syncdata" };

                        if (!string.IsNullOrEmpty(accessToken) && lstHub.Select(i => path.StartsWithSegments(i)).Any(i => i)
                            //(path.StartsWithSegments("/qrhostpc") || path.StartsWithSegments("/syncdata")
                            )
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                    //OnTokenValidated = async context =>
                    // {
                    //    // var currentUser = context.HttpContext.RequestServices.GetRequiredService<ICurrentUser>();
                    //    var service = context.HttpContext.RequestServices;
                    //     var claims = context.Principal.Claims.ToList();
                    //     var identity = new IdentityUserModel
                    //     {
                    //         UserId = Convert.ToInt32(claims.FirstOrDefault(x => x.Type == "user_id")?.Value),
                    //         UserName = context.HttpContext.User.Identity.Name,
                    //         FullName = claims.FirstOrDefault(x => x.Type == "full_name")?.Value,
                    //         CompanyId = Convert.ToInt32(claims.FirstOrDefault(x => x.Type == "company_id")?.Value),
                    //         CompanyPackageId = Convert.ToInt32(claims.FirstOrDefault(x => x.Type == "company_pack_id")?.Value),
                    //         OrgId = Convert.ToInt32(claims.FirstOrDefault(x => x.Type == "org_id")?.Value)
                    //     };

                    // }
                };
            });

            //services.AddAutoMapper();

            services.AddMemoryCache();
            //services.AddMemoryCache(opt =>
            //{
            //    opt.SizeLimit = 10240;
            //});
            // No need. Adding controller already for json format
            //services
            //    //.AddMvc( 
            //    //options =>
            //    //{
            //    //    var jsonInputFormatter = options.InputFormatters.OfType<JsonInputFormatter>().First();
            //    //    jsonInputFormatter.SupportedMediaTypes.Add("multipart/form-data");
            //    //}
            //    //)
            //    //.AddJsonOptions(options =>
            //    //{
            //    //    options.SerializerSettings.ContractResolver = new JsonLowercaseContractResolver();
            //    //    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            //    //})
            //    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true; // Allow client post null data and generate data in API
            });
            //AppSettings
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            services.Configure<Jwt>(Configuration.GetSection(nameof(Jwt)));
            services.Configure<FileServerSettings>(Configuration.GetSection(nameof(FileServerSettings)));
            services.Configure<DataColor_FileServerSettings>(Configuration.GetSection(nameof(DataColor_FileServerSettings)));

            services.AddSwaggerGen(
               options =>
               {

                   var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                   var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                   options.IncludeXmlComments(xmlPath);

                   options.SwaggerDoc(
                       "auth",
                       new OpenApiInfo
                       {
                           Title = $"ERP Solution Management API ",

                           Description = "This API provides functions processing for ERP AtMan service",
                           Contact = new OpenApiContact
                           {
                               Name = "AtMan",
                               //Url=new Uri(Uri),
                               Email = "AtmanEuler.com",
                           },
                           Version = "v1"
                           //License = new License()
                           //{
                           //    Name = "Name",
                           //    Url = @"Url"
                           //},
                           //TermsOfService = @"TermsOfService"
                       });


                   options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                   {
                       Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                       Name = "Authorization",
                       In = ParameterLocation.Header,
                       Type = SecuritySchemeType.ApiKey,
                       Scheme = "Bearer"
                   });

                   options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                   options.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
                   options.SwaggerDoc("kmsjobphoto_mobile", new OpenApiInfo { Title = "KMS Job Photo API", Version = "KMS" });
                   options.SwaggerDoc("systemmaster", new OpenApiInfo { Title = "API for System", Version = "SystemMaster" });
               });
           

            services.AddSingleton(provider => Configuration);

            services.ConfigureLoggerService();
            services.AddRegisterService();
           // var redisConnection = Configuration.GetConnectionString(nameof(ConnectionSetting.RedisConnection));
 


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            //loggerFactory.AddLog4Net();
            AppSettings appSettings = Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            string path = Path.Combine(env.ContentRootPath, appSettings.UploadRootPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (Directory.Exists(path))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, appSettings.UploadRootPath)),
                    RequestPath = appSettings.UploadRootDomain
                });
            }

            string downloadsPath = Environment.GetEnvironmentVariable("DOWNLOADS_PATH");
            if (string.IsNullOrWhiteSpace(downloadsPath))
            {
                downloadsPath = Path.Combine(env.ContentRootPath, "downloads");
            }
            if (Directory.Exists(downloadsPath))
            {
 
                var downloadsContentTypes = new FileExtensionContentTypeProvider();
                downloadsContentTypes.Mappings[".apk"] = "application/vnd.android.package-archive";

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(downloadsPath),
                    RequestPath = "/downloads",
                    ContentTypeProvider = downloadsContentTypes,
                    ServeUnknownFileTypes = true
 
                });
            }

            if (!string.IsNullOrWhiteSpace(appSettings.AuditImageRootPath) &&
                Directory.Exists(appSettings.AuditImageRootPath))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(appSettings.AuditImageRootPath),
                    RequestPath = "/audit/img"
                });
            }

            //app.UseForwardedHeaders();
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseDatabaseErrorPage();
            //}
            //else
            //{
            //    app.UseStatusCodePagesWithReExecute("/Home/Error");
            //}

            app.UseMiddleware<ApiErrorHandlingMiddleware>();


            //app.UseCors("policy");
            //app.UseAuthentication();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            app.UseAuthentication();
            app.UseAuthorization();
            //Need Author First
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<TokenExpiredMiddleWare>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers = new List<OpenApiServer>();
                    if (appSettings.Surfix != string.Empty)
                    {
                        swagger.Servers.Add(new OpenApiServer { Url = $"{appSettings.Surfix}" });
                    }

                });
            });
            app.UseSwaggerUI(c =>
            {
                // Expose Swagger UI at the default "/swagger" path so that launching the
                // application in IIS Express (which automatically opens that URL) works.
                c.RoutePrefix = "swagger";
                var basePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"/{basePath}{appSettings.Surfix}/swagger/auth/swagger.json", "Auth API");
                c.SwaggerEndpoint($"/{basePath}{appSettings.Surfix}/swagger/kmsjobphoto_mobile/swagger.json", "KMS Job Photo API");
                c.SwaggerEndpoint($"/{basePath}{appSettings.Surfix}/swagger/systemmaster/swagger.json", "System API");
               

            });
        }

    }

    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            //Dapper

            //services.AddTransient<IDbConnection>(options => new SqlConnection(conn));
            //string msConnection = configuration.GetConnectionString(nameof(ConnectionSetting.MsSQLConnection));
            string amtConnection = configuration.GetConnectionString(nameof(ConnectionSetting.OracleConnection));
            //string erpConnection = configuration.GetConnectionString(nameof(ConnectionSetting.ErpConnection));
            //string amtConnection = configuration.GetConnectionString(nameof(ConnectionSetting.AmtConnection));

            services.Configure<ConnectionSetting>(configuration.GetSection("ConnectionStrings"));
 
            services.AddDbContext<AmtOracleContext, AmtContext>((options) =>
            {
                options.UseOracle(amtConnection,
                    oracleOptionsAction: sqlOptions =>
                    {
                        //sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name)
                    .CommandTimeout(420)//< 1 Min is OK, if longer u need check your query
                                        // Thay thế "11" (string) bằng giá trị enum thích hợp.
                                        // Giả sử bạn đang dùng Devart.Data.Oracle.EFCore hoặc phiên bản Oracle.EntityFrameworkCore cũ.
                    .UseOracleSQLCompatibility(Microsoft.EntityFrameworkCore.OracleSQLCompatibility.DatabaseVersion19);
                    });
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }, ServiceLifetime.Scoped);

            services.AddDbContext<LogContext>((options) =>
            {
                options.UseOracle(amtConnection,
                    oracleOptionsAction: sqlOptions =>
                    {
                        //sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name)
                        .CommandTimeout(420)//< 1 Min is OK, if longer u need check your query
                        .UseOracleSQLCompatibility(Microsoft.EntityFrameworkCore.OracleSQLCompatibility.DatabaseVersion19);
                    });
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }, ServiceLifetime.Scoped);

            return services;
        }
    }
}
