using CommHost.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CommHost.Web {

    public class WebServer {

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
        }

        public void Configure(IConfiguration config, IApplicationBuilder app, IWebHostEnvironment env, ILogger<WebServer> logging) {

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseMiddleware<UnhandledExceptionMiddleware>();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

        }

    }

}
