// Copyright 2007-2013 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
using System;
using Serilog;
using Topshelf;
using Serilog.Events;
using Hangfire.Logging;
using Hangfire.Logging.LogProviders;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Configuration;
using System.IO;
namespace Hangfire.NetCoreServices
{
   
    public  class Program
    {

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();


        public static int Main(string[] args)
        {
            var a = Directory.GetCurrentDirectory();

            return (int)HostFactory.Run(x =>
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)//debug
                    .Enrich.FromLogContext()
                    //.WriteTo.MSSqlServer(Configuration["SerilogLogConnection"], "Logs", columnOptions: columnOptions, autoCreateSqlTable: true)
                    .WriteTo.ColoredConsole()//debug
                    .WriteTo.RollingFile(AppDomain.CurrentDomain.BaseDirectory + @"\logs\Hangfire.log",
                        fileSizeLimitBytes: 1_000_000,
                        shared: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(5))
                    .CreateLogger();

                LogProvider.SetCurrentLogProvider(new SerilogLogProvider());



                x.UseSerilog();

                x.UseAssemblyInfoForServiceInfo();

                bool throwOnStart = false;
                bool throwOnStop = false;
                bool throwUnhandled = false;


                string str = AppDomain.CurrentDomain.BaseDirectory;

                x.Service(settings => new SampleService(throwOnStart, throwOnStop, throwUnhandled, args), s =>
                {
                    s.BeforeStartingService(_ => Console.WriteLine("BeforeStart"));
                    s.BeforeStoppingService(_ => Console.WriteLine("BeforeStop"));
                });

                x.SetStartTimeout(TimeSpan.FromSeconds(10));
                x.SetStopTimeout(TimeSpan.FromSeconds(10));

                x.EnableServiceRecovery(r =>
                {
                    r.RestartService(3);
                    r.RunProgram(7, "ping aliyun.com");
                    r.RestartComputer(5, "message");

                    r.OnCrashOnly();
                    r.SetResetPeriod(2);
                });

                x.AddCommandLineSwitch("throwonstart", v => throwOnStart = v);
                x.AddCommandLineSwitch("throwonstop", v => throwOnStop = v);
                x.AddCommandLineSwitch("throwunhandled", v => throwUnhandled = v);

                x.OnException((exception) =>
                {
                    Console.WriteLine("Exception thrown - " + exception.Message);
                });
            });
        }

        void SansInterface()
        {
            HostFactory.New(x =>
            {
                // can define services without the interface dependency as well, this is just for
                // show and not used in this sample.
                x.Service<SampleSansInterfaceService>(s =>
                {
                    s.ConstructUsing(() => new SampleSansInterfaceService());
                    s.WhenStarted(v => v.Start());
                    s.WhenStopped(v => v.Stop());
                });
            });
        }


        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseKestrel()
                .UseUrls("http://*:5100")
                .UseStartup<Startup>();
    }
}