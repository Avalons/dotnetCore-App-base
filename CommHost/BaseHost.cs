using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace CommHost {

    public class BaseHost {

        #region webserver
        public class WebServer {

            public void Configure(IConfiguration config, IApplicationBuilder app, IWebHostEnvironment env, ILogger<WebServer> logging) {

                logging.LogInformation($"EnvironmentName:{env.EnvironmentName}");
                logging.LogInformation($"WebPort:{config["WebPort"]}");

                app.UseRouting()
                    .UseEndpoints(endpoints => {
                        endpoints.MapGet("/", async context => {
                            await context.Response.WriteAsync("Hello World!");
                        });
                    });

            }

        }
        #endregion

        private readonly IHost _host;

        public async Task runAsync() {
            await _host.RunAsync();
        }

        public async Task stopAsync() {
            await _host.StopAsync();
        }

        public BaseHost(string[] args) {

            var builder = Host.CreateDefaultBuilder(args);
            var logger = LogManager.GetCurrentClassLogger();

            #region get config
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables(prefix: "DOTNET_")
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddCommandLine(args)
                .Build();

            var webPort = int.Parse(config["WebPort"]);
            #endregion

            #region nlog
            LogManager.Configuration = new XmlLoggingConfiguration("nlog." + env + ".config", true);
            builder.ConfigureLogging(loggingBuilder => {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog(config);
            });
            #endregion

            #region set global limit
            ThreadPool.GetMaxThreads(out var cur_avi_th, out var cur_avi_other_th);
            ThreadPool.SetMinThreads(cur_avi_th, cur_avi_other_th);
            ThreadPool.SetMaxThreads(cur_avi_th, cur_avi_other_th);
            logger.Info($"SetMaxThreads:{cur_avi_th},{cur_avi_other_th}");

            System.Net.ServicePointManager.DefaultConnectionLimit = 999999;
            System.Net.ServicePointManager.MaxServicePoints = 999999;
            System.Net.ServicePointManager.Expect100Continue = false;
            #endregion

            builder.ConfigureWebHostDefaults(webHostBuilder => {

                webHostBuilder.ConfigureKestrel(opt => {
                    opt.Limits.MinRequestBodyDataRate = null;
                    opt.ListenAnyIP(webPort);
                });

                webHostBuilder.UseStartup<WebServer>();

            });

            _host = builder.Build();

        }

    }

}
