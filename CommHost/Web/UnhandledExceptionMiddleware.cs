using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace CommHost.Web {

    public class UnhandledExceptionMiddleware {
        private readonly RequestDelegate _next;

        public UnhandledExceptionMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context) {

            try {
                await _next(context);
            }
            catch (Exception e) {

                await context.Response.WriteAsync($"unhandled exception found x_x , {e.Message}", Encoding.UTF8);

                return;

            }

        }
    }

}
