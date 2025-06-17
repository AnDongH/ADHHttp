using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServer.Middleware {
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ReqPerSecMiddleware {
        
        private readonly RequestDelegate _next;
        private static int _requestCount = 0;
        //private static int _responseCount = 0;
        private static readonly object _lock = new object();
       // private static readonly object _reslock = new object();
        private static Timer _timer = new Timer(ResetCount, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        //private Timer _restimer;

        public ReqPerSecMiddleware(RequestDelegate next) {
            
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {

            lock (_lock) {
                _requestCount++;
            }

            await _next(context);
        
        }

        private static void ResetCount(object state) {
            lock (_lock) {
                Console.WriteLine($"[Request] 초당 들어온 요청 수: {_requestCount}");
                _requestCount = 0;
            }
        }

       //private void ResetResCount(object state) {
       //    lock (_reslock) {
       //        Console.WriteLine($"[Response] 초당 처리된 요청 수: {_responseCount}");
       //        _responseCount = 0;
       //    }
       //}
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ReqPerSecMiddlewareExtensions {
        public static IApplicationBuilder UseReqPerSecMiddleware(this IApplicationBuilder builder) {
            return builder.UseMiddleware<ReqPerSecMiddleware>();
        }
    }
}
