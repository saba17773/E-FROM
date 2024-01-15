using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Web.UI.Contexts;
using Web.UI.DataAccess.Contexts;
using Web.UI.Interfaces;
using Web.UI.Services;
using Web.UI.Services.CovidTracker;
using Web.UI.Services.CovidTracker.Handler;
using Web.UI.Services.CovidTracker.Interface;
using Web.UI.Services.DataTable.Handler;
using Web.UI.Services.DataTable.Interface;

namespace Web.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-Us");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-Us") };
                options.RequestCultureProviders.Clear();
            });

            services
                .AddRazorPages()
                .AddRazorRuntimeCompilation();

            services.AddCors(options =>
            {
                options.AddPolicy("CORSPolicy", builder => builder
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowAnyMethod());
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Config:BaseUrl"],
                    ValidAudience = Configuration["Config:BaseUrl"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddDbContextPool<NewContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default")).EnableSensitiveDataLogging());

            services.AddSingleton<IDatabaseContext, DatabaseContext>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddTransient<IHelperService, HelperService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IDatatableService, DatatableService>();
            services.AddTransient<ICreditControlService, CreditControlService>();

            // Services
            services.AddTransient<IDtService, DtService>();
            services.AddScoped<IVaccineService, VaccineService>();

            //services.AddMvc();
            //services.AddDistributedMemoryCache();
            //services.AddSession();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.Use((context, next) =>
            {
                string token = "NOT_AUTHORIZED";

                if (context.Request.Cookies.ContainsKey(Configuration["Jwt:Name"]) == true)
                {
                    token = context.Request.Cookies[Configuration["Jwt:Name"]];
                    context.Request.Headers.Add("Authorization", $"Bearer {token}");
                }

                return next();
            });

            app.UseRequestLocalization();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseStatusCodePagesWithRedirects("/Error?statusCode={0}");
            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

        }
    }
}
