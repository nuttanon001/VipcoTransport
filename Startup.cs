using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SpaServices.Webpack;

using VipcoTransport.Models;
using VipcoTransport.Classes;
using VipcoTransport.ViewModels;
using VipcoTransport.Services.Classes;
using VipcoTransport.Services.Interfaces;

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebApplicationBasic
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        private SymmetricSecurityKey _signingKey =>
           new SymmetricSecurityKey(Encoding.ASCII.GetBytes("vipcotransport2560"));
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNodeServices();// this is in package Microsoft.AspNetCore.NodeServices
            // Add framework services.
            services.AddMvc();
            services.AddOptions();
            // Add AutoMapper
            services.AddAutoMapper(typeof(Startup));

            // Use policy auth.
            services.AddAuthorization(options =>
            {
                options.AddPolicy("SystemUser", policy => policy.RequireClaim("Administrator", "admin"));
            });

            // Add ApplicationDbContext.
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(Configuration.GetConnectionString("TransportDataBase")));

            // Add transient
            services.AddTransient<ICarRepository, CarRepository>();
            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            services.AddTransient<ITransportRepository, TransportRepository>();
            services.AddTransient<ITransportRequestRepository, TransportRequestRepository>();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            //var tokenValidationParameters = new TokenValidationParameters
            //{
            //    ValidateIssuer = true,
            //    ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

            //    ValidateAudience = true,
            //    ValidAudiences = Auds,

            //    ValidateIssuerSigningKey = true,
            //    IssuerSigningKey = _signingKey,

            //    RequireExpirationTime = false,
            //    ValidateLifetime = false,

            //    ClockSkew = TimeSpan.Zero
            //};

            //app.UseJwtBearerAuthentication(new JwtBearerOptions
            //{
            //    AutomaticAuthenticate = true,
            //    AutomaticChallenge = true,
            //    TokenValidationParameters = tokenValidationParameters,
            //});
            var jwtAppSettingOptions2 = Configuration.GetSection(nameof(JwtIssuerOptions2));
            JwtIssuerOptions2.Issuer = Configuration.GetSection(JwtIssuerOptions2.Nameof);
            IEnumerable<string> Auds = new List<string>() {
                jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience2)],
                jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience3)] };
            /// <summary>
            /// "aud" (Audience) Claim
            /// </summary>
            /// <remarks>The "aud" (audience) claim identifies the recipients that the JWT is
            ///   intended for.  Each principal intended to process the JWT MUST
            ///   identify itself with a value in the audience claim.  If the principal
            ///   processing the claim does not identify itself with a value in the
            ///   "aud" claim when this claim is present, then the JWT MUST be
            ///   rejected.  In the general case, the "aud" value is an array of case-
            ///   sensitive strings, each containing a StringOrURI value.  In the
            ///   special case when the JWT has one audience, the "aud" value MAY be a
            ///   single case-sensitive string containing a StringOrURI value.  The
            ///   interpretation of audience values is generally application specific.
            ///   Use of this claim is OPTIONAL.</remarks>
            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.Audience2 = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience2)];
                options.Audience3 = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience3)];

                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            IEnumerable<string> Auds = new List<string>() {
                jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience2)],
                jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience3)] };
            #region Parameters1

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudiences = Auds,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = false,

                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters,
            });

            #endregion

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });

                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<ApplicationContext>().Database.Migrate();
                    serviceScope.ServiceProvider.GetService<ApplicationContext>().EnsureSeedData();
                }
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
