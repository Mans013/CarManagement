using System;
using System.Collections.Generic;
using CarManagement.Manager;
using CarManagement.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQManagement;



namespace CarManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //           var appSettingsSection = Configuration.GetSection("AppSettings");
            //           services.Configure<AppSettings>(appSettingsSection);

            services.AddTransient<RabbitMQMessagePublisher>((svc) =>
            {
                return new RabbitMQMessagePublisher("localhost", "CarChamp");
            });
            services.AddTransient<RabbitMQMessageHandler>((svc) =>
            {
                var list = new List<string>
                {
                    "#.maintenance.#",
                    "#.advertisement.#"
                };
                return new RabbitMQMessageHandler("localhost", "CarChamp", "carManagement", list);
            });
            services.AddHostedService<CarManager>();
            services.AddSingleton<CarManagementService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
