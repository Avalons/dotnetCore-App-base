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

        private IHost _host;
        private IHostBuilder _builder;
        private int _webPort;

        public async Task runAsync<TWebServer>() {

            _builder.ConfigureWebHostDefaults(webHostBuilder => {

                webHostBuilder.ConfigureKestrel(opt => {
                    opt.Limits.MinRequestBodyDataRate = null;
                    opt.ListenAnyIP(_webPort);
                });

                webHostBuilder.UseStartup<WebServer>();

            });

            _host = _builder.Build();
            await _host.RunAsync();
        }

        public async Task stopAsync() {
            await _host.StopAsync();
        }

        public BaseHost(string[] args) {

            _builder = Host.CreateDefaultBuilder(args);
            var logger = LogManager.GetCurrentClassLogger(); 

            #region get config
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            LogManager.Configuration = new XmlLoggingConfiguration("nlog." + env + ".config", true);
            logger.Info($"EnvironmentName:{env}");

            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables(prefix: "DOTNET_")
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddCommandLine(args)
                .Build();

            _webPort = int.Parse(config["WebPort"]);
            logger.Info($"WebPort:{_webPort}");
            #endregion

            #region nlog
           
            _builder.ConfigureLogging(loggingBuilder => {
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

        }

    }

}
