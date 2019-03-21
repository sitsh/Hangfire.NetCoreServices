using System;
using System.Collections.Generic;
using System.Text;
using Hangfire.NetCoreServices.Common;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hangfire.NetCoreServices.Extensions
{
    public static class HealthChecksExtensions
    {
        /// <summary>
        /// 配置 api Swagger  Startup 辅助类
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHealthChecksService(this IServiceCollection services, IConfiguration configuration)
        {
            /*
              *HealthChecks
             * https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks
              */
            services.AddHealthChecks()
                .AddHangfire(opt =>
                {
                    opt.MaximumJobsFailed = 11;
                }, name: "hangfire60")
                .AddSqlServer(connectionString: configuration["Hangfire:SqlConnectionString"], name: "Hangfire.SqlServer60")

                .AddCheck<RandomHealthCheck>("random");//随机 测试 健康用例。正式版删除。


            return services;
        }
        /// <summary>
        /// 配置 api Swagger  Startup 辅助类
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHealthChecksService(this IApplicationBuilder app)
        {

            //HealthChecks
            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true
            });

            app.UseHealthChecks("/healthz", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            return app;
        }

    }
}
