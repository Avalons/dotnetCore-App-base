using System;
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

            decimal 跑道 = 1200m;
            decimal 甲速度 = 10m;
            decimal 乙速度 = 4m;

            decimal 乙当前耗时 = 0m;
            decimal 甲当前耗时 = 0m;
            decimal 甲距离 = 0m;
            decimal 乙距离 = 0m;

            decimal 甲距离_in_跑道 = 0m;
            decimal 乙距离_in_跑道 = 0m;

            decimal 乙圈数 = 0m;

            int 圈数 = 1;

            while (true) {

                乙距离 = 乙速度 * 乙当前耗时;
                甲距离 = 甲速度 * 甲当前耗时;

                if (甲距离 == 跑道) {
                    甲距离_in_跑道 = 0;
                    if (圈数 == 1)
                        圈数++;
                }
                else {
                    甲距离_in_跑道 = 甲距离 % 跑道;
                }

                if (乙距离 == 跑道) {
                    乙距离_in_跑道 = 0;
                    if (圈数 == 1)
                        圈数++;
                }
                else {
                    乙距离_in_跑道 = 乙距离 % 跑道;
                }

                Console.WriteLine($"甲当前耗时:{甲当前耗时}秒,乙当前耗时:{乙当前耗时}秒,甲距离:{甲距离},乙距离:{乙距离},乙距离_in_跑道:{乙距离_in_跑道},甲距离_in_跑道:{甲距离_in_跑道}");

                

                if (乙当前耗时 >= 10) {
                    甲当前耗时++;
                    乙当前耗时++;
                }
                else {
                    乙当前耗时++;
                }
            }

            return;

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
