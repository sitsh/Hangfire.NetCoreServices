using System;
using System.Collections.Generic;
using System.Text;
using Hangfire.NetCoreServices.Extensions;
using Hangfire.NetCoreServices.Impl;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hangfire.NetCoreServices
{
    public class Startup
    {

        public static IServiceCollection Services { set; get; }
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            //Configuration = builder.Build();
            //Services = services;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddHangfire(configuration =>
            {
                // 注入 HangFire 服务器数据库配置
                string connectionString = Configuration["Hangfire:SqlConnectionString"];
                //configuration.UseSqlServerStorage(@"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=True;Initial Catalog=Hangfire.WindowsServiceApplication;",
                configuration.UseSqlServerStorage(connectionString,
                    new SqlServerStorageOptions
                    {
                        QueuePollInterval = TimeSpan.FromSeconds(1),
                        PrepareSchemaIfNecessary = false,
                        SchemaName = "HangFire"
                    });
            });
            //加入 HealthChecks 健康监控服务
            services.AddHealthChecksService(Configuration);
            //hangfire 业务job 接口注入
            services.AddScoped<ICosJob, CosJob>();
            //注入其他 DB等等 ....

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWelcomePage("/");
            }
            else
            {
                app.UseHsts();
            }
            var options = new BackgroundJobServerOptions
            {
                // This is the default value
                WorkerCount = Environment.ProcessorCount * 1,
                //Queues = new[] { "critical", "default" } //配置 读取订阅队列的顺序
            };

            //启用 Hangfire 仪表板
            app.UseHangfireDashboard();
            app.UseHangfireServer(options);

            //使用 HealthChecks 健康监控服务
            app.UseHealthChecksService();
        }

       
    }
}
