using FluentMigrator.Runner;
using KLog.Api.Config;
using KLog.Api.Core.Authentication;
using KLog.Api.Hubs;
using KLog.Api.Services;
using KLog.DataModel.Context;
using KLog.DataModel.Migrations;
using KLog.DataModel.Migrations.Releases.Release_001;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KLog.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // ************************************ //
            //        Make settings available       //
            // ************************************ //
            AppSettings settings = new AppSettings();
            SecuritySettings securitySettings = new SecuritySettings();
            Configuration.GetSection("AppSettings").Bind(settings);
            Configuration.GetSection("SecuritySettings").Bind(securitySettings);

            services.Configure<SecuritySettings>(Configuration.GetSection("SecuritySettings"));

            // ************************************ //
            //            Database Setup            //
            // ************************************ //
            services.AddDbContext<KLogContext>(options =>
                options.UseSqlServer(settings.ConnectionStrings["KLog.Db"]));

            services.AddMigrations(
                settings.ConnectionStrings["KLog.Db"],
                new List<Type> { typeof(Release_001) }
            );

            KLogContext.SetTriggers();

            // ************************************ //
            //              CORS Setup              //
            // ************************************ //
            if(settings.CORS != null)
                services.AddCors(options =>
                {
                    options.AddPolicy(
                        "CorsPolicy",
                        builder => builder.WithOrigins(settings.CORS)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                    );
                });

            // ************************************ //
            //   Authentication and Authorization   //
            // ************************************ //

            services.AddAuthentication()
                .AddJwtBearer("JWT", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = securitySettings.Issuer,
                        ValidAudience = securitySettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(securitySettings.SecretKey))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = HandleMessageReceived
                    };
                })
                .AddScheme<ApiKeyOptions, ApiKeyHandler>("ApiKey", null);

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("JWT", "ApiKey")
                    .Build();
            });

            // ************************************ //
            //                SignalR               //
            // ************************************ //
            services.AddSignalR()
                .AddNewtonsoftJsonProtocol();

            // ************************************ //
            //          Additional Services         //
            // ************************************ //
            services.AddTransient<IAuthenticationService, AuthenticationService>();

            services.AddHttpContextAccessor();

            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "KLog.Api", Version = "v1" });

                string xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider)
        {
            // ************************************ //
            //           Run One-time Setup         //
            // ************************************ //
            using (IServiceScope scope = provider.CreateScope())
            {
                IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateUp();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "KLog.Api v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseWebSockets();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<LogHub>("/logHub");
            });
        }

        public Task HandleMessageReceived(MessageReceivedContext context)
        {
            StringValues accessToken = context.Request.Query["access_token"];
            PathString path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/logHub"))
                context.Token = accessToken;

            return Task.CompletedTask;
        }
    }
}
