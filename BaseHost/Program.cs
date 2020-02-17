using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BaseHost {

    public class Program {
        public static async Task Main(string[] args) {

            await Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging => logging.AddConsole())
                .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<WebserverStartup>())
                .Build()
                .RunAsync();

        }
    }

    public class WebserverStartup {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {

            app.UseRouting()
                .UseEndpoints(endpoints => {
                    endpoints.MapGet("/", async context => {
                        await context.Response.WriteAsync("Hello World!");
                    });
                });

        }
    }

}
