using FluentMigrator.Runner;
using KLog.Api.Config;
using KLog.Api.Core.Authentication;
using KLog.Api.Services;
using KLog.DataModel.Context;
using KLog.DataModel.Migrations;
using KLog.DataModel.Migrations.Releases.Release_001;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
            Configuration.GetSection("AppSettings").Bind(settings);

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
            //   Authentication and Authorization   //
            // ************************************ //

            services.AddAuthentication()
                .AddScheme<ApiKeyOptions, ApiKeyHandler>("ApiKey", null);

            // ************************************ //
            //          Additional Services         //
            // ************************************ //
            services.AddTransient<ISecurityService, SecurityService>();

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

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "KLog.Api v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
