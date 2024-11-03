using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Spectre.API.JwtAuthentication;
using Spectre.API.Utilities;
using Spectre.Core.Interfaces;
using Spectre.Core.Models;
using Spectre.Core.Repositories;
using System;
using System.Linq;
using System.Text;

namespace Spectre.API
{
    public class Startup
    {
        [Obsolete]
        public Microsoft.Extensions.Hosting.IHostingEnvironment Hosting;

        [Obsolete]
        public Startup(IConfiguration configuration, Microsoft.Extensions.Hosting.IHostingEnvironment hosting)
        {
            Configuration = configuration;
            Hosting = hosting;

            var builder = new ConfigurationBuilder().SetBasePath(System.IO.Directory.GetCurrentDirectory()).AddJsonFile(Configuration.GetConnectionString("DefaultConnection"));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //   services.AddDbContext<SpectreDBContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<SpectreDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), o =>
            {
                o.EnableRetryOnFailure();
            }));

            services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<SpectreDBContext>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JwtAuthentication:JwtIssuer"],
                    ValidAudience = Configuration["JwtAuthentication:JwtIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtAuthentication:JwtKey"])),
                };

            });
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ILookupRepository, LookupRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRegisterationRequestRepository, RegisterationRequestRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IScocioEconomicRepository, ScocioEconomicRepository>();
            services.AddScoped<IAwardRepository, AwardRepository>();
            services.AddTransient<ICountryRepository, CountryRepository>();
            services.AddTransient<ILatestNewsRepository, LatestNewsRepository>();
            services.AddTransient<ISystemSettingsRepository, SystemSettingsRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IBandRepository, BandRepository>();
            services.AddTransient<IPackageRepository, PackageRepository>();
            services.AddTransient<IPageRepository, PageRepository>();
            services.AddTransient<ICompanyPreRegistrationRepository, CompanyPreRegistrationRepository>();
            services.AddTransient<IIconRepository, IconRepository>();
            services.AddTransient<ICompanyPackageRepository, CompanyPackageRepository>();
            services.AddTransient<IParamRepository, ParamRepository>();
            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<IHomePageReposiotory, HomePageReposiotory>();
            services.AddTransient<IContactUsRepository, ContactUsRepository>();
            services.AddTransient<IStatisticRepository, StatisticRepository>();
            services.AddTransient<ISavedFilterRepository, SavedFilterRepository>();
            services.AddTransient<ITemplateFilterRepository, TemplateFilterRepository>();
            services.AddTransient<IFeatureRepository, FeatureRepository>();

            services.AddTransient<IFQARepository, FQARepository>();
            services.AddTransient<IHelpServicesRepository, HelpServicesRepository>();
            services.AddTransient<IGlossaryRepository, GlossaryRepository>();
            services.AddTransient<IHelpUsingRepository, HelpUsingRepository>();
            services.AddTransient<IRegionRepository, RegionRepository>();
            services.AddTransient<Spectre.Core.RepositoryHandler.ILogger, Spectre.Core.RepositoryHandler.Logger>();
            services.AddAutoMapper(typeof(Startup));
            services.AddControllers();
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddMvc();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
            });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Spectre API", Version = "v2" });
                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                });

                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                //////Add Operation Specific Authorization///////
                c.OperationFilter<AddAuthHeaderOperationFilter>();
            });
            services.AddTransient<TokenGenerator>();
            services.AddTransient<Helpers>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {


            app.UseDeveloperExceptionPage();


            app.UseAuthentication();
            app.UseMiddleware<UnauthorizedResponse>();

            // added to stop swagger in production / release
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint(Configuration["SettingKeys:SwaggerEndPoint"], "Spectre.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
                      "Origin, X-Requested-With, Content-Type, Accept");
                },

            });



            app.UseCors("MyPolicy");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Website");
            });
        }

    }
}
