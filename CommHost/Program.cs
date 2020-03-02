using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace CommHost {

    
    #region webserver
    public class WebServer {

        public void Configure(IConfiguration config, IApplicationBuilder app, IWebHostEnvironment env, ILogger<WebServer> logging) {

            app.UseRouting()
                .UseEndpoints(endpoints => {
                    endpoints.MapGet("/", async context => {
                        await context.Response.WriteAsync("Hello World!");
                    });
                });

        }

    }
    #endregion
    
    class Program {

        private static async Task Main(string[] args) {
            var host = new BaseHost(args);
            await host.runAsync<WebServer>();
        }

    }

}
