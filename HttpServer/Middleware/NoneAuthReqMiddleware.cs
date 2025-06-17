using HttpServer.Repository;
using HttpServer.Services;
using MemoryPack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADHNetworkShared.Crypto;
using ADHNetworkShared.Protocol.DTO;
using ADHNetworkShared.Protocol;
using Microsoft.Extensions.Options;
using MemoryPack.Compression;

namespace HttpServer.Middleware {
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class NoneAuthReqMiddleware {

        private readonly ILogger<NoneAuthReqMiddleware> _logger;
        private readonly IOptions<KeyOption> _keyOption;
        private readonly RequestDelegate _next;

        public NoneAuthReqMiddleware(RequestDelegate next, ILogger<NoneAuthReqMiddleware> logger, IOptions<KeyOption> keyOption) {
            
            _keyOption = keyOption;
            _logger = logger;
            _next = next;

        }

        public async Task Invoke(HttpContext context) {

            HttpRequest request = context.Request;

            try {

                // using var inStream = request.Body;
                // using var stream = new MemoryStream();
                //
                // await inStream.CopyToAsync(stream);
                // stream.Position = 0;
                // byte[] buffer = stream.ToArray();
                
                var reader = request.BodyReader;
                var completeMessage = new List<byte>();
        
                while (true)
                {
                    var readResult = await reader.ReadAsync();
                    var buffer = readResult.Buffer;
        
                    completeMessage.AddRange(buffer.ToArray());
                    
                    reader.AdvanceTo(buffer.End);
                    
                    if (readResult.IsCompleted || readResult.IsCanceled)
                        break;
                }
                
                byte[] plainByte = ECIES.DecryptECIES(completeMessage.ToArray(), ECIES.RestorePrivateBytesToKey(Convert.FromBase64String(_keyOption.Value.PrivateKey)), out var senderPubKey);

                var req = MemoryPackSerializer.Deserialize<ProtocolReq>(plainByte);

                context.Items[nameof(ProtocolReq)] = req;
                context.Items["SenderPublicKey"] = senderPubKey;

                await _next(context);

            } catch (Exception ex) {

                _logger.LogError($"Error in None Auth Method {ex.ToString()}");

                var errorResponse = MemoryPackSerializer.Serialize(new BasicProtocolRes {
                    Result = ErrorCode.ServerUnhandleException
                });

                // await using (var outStream = context.Response.Body) {
                //     await outStream.WriteAsync(errorResponse);
                // }

                var writer = context.Response.BodyWriter;
            
                await writer.WriteAsync(errorResponse);
                await writer.FlushAsync();
                
                return;

            }

        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class NoneAuthReqMiddlewareExtensions {
        public static IApplicationBuilder UseNoneAuthReqMiddleware(this IApplicationBuilder builder) {
            return builder.UseMiddleware<NoneAuthReqMiddleware>();
        }
    }
}
