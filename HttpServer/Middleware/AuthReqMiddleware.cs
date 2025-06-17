using HttpServer.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HttpServer.DAO;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.Services;
using ADHNetworkShared.Protocol;
using System.IO;
using ADHNetworkShared.Crypto;
using MemoryPack;
using HttpServer.Repository.Interface;

namespace HttpServer.Middleware {
    public class AuthReqMiddleware {

        private readonly ILogger<AuthReqMiddleware> _logger;
        private readonly IMemoryDB_Test _memoryDB;
        private readonly RequestDelegate _next;
        private readonly ILoginAuthenticationService _loginAuthenticationService;

        public AuthReqMiddleware(RequestDelegate next, IMemoryDB_Test memoryDB, ILogger<AuthReqMiddleware> logger, ILoginAuthenticationService loginAuthenticationService) {
            
            _memoryDB = memoryDB;
            _next = next;
            _logger = logger;
            _loginAuthenticationService = loginAuthenticationService;
            
        }

        
        public async Task Invoke(HttpContext context) {

            try {

                var request = context.Request;
                var errorCode = ErrorCode.None;
                
                // using var inStream = request.Body;
                // using var stream = new MemoryStream();
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
                
                var data = MemoryPackSerializer.Deserialize<ClientEncryptedData>(completeMessage.ToArray());
                (errorCode, var key) = await _memoryDB.GetClientKeyAsync(data.RedisKey);

                if (errorCode != ErrorCode.None)
                {
                    var errorResponse = MemoryPackSerializer.Serialize(new BasicProtocolRes
                    {
                        Result = errorCode
                    });

                    // await using (var outStream = context.Response.Body) {
                    //     await outStream.WriteAsync(errorResponse);
                    // }

                    var writer = context.Response.BodyWriter;

                    await writer.WriteAsync(errorResponse);
                    await writer.FlushAsync();

                    return;
                }

                var authReq =
                    MemoryPackSerializer.Deserialize<ProtocolReq>(AES.DecryptAES(data.Data, key, data.IV)) as
                        AuthProtocolReq;

                // 요청에 token이 있는지 검사하고 있다면 저장 -> 로그인 정보를 같이 보내주는 것
                (errorCode, var token) = _loginAuthenticationService.IsTokenNotExistOrReturnToken(authReq);

                if (errorCode != ErrorCode.None)
                {
                    var errorResponse = MemoryPackSerializer.Serialize(new BasicProtocolRes
                    {
                        Result = errorCode
                    });

                    // await using (var outStream = context.Response.Body) {
                    //     await outStream.WriteAsync(errorResponse);
                    // }

                    var writer = context.Response.BodyWriter;

                    await writer.WriteAsync(errorResponse);
                    await writer.FlushAsync();

                    return;
                }

                //user_id가 있는지 검사하고 있다면 저장 -> 존재하는 유저인지 확인
                (errorCode, var user_id) = _loginAuthenticationService.IsUserIDNotExistOrReturnUserID(authReq);

                if (errorCode != ErrorCode.None)
                {
                    var errorResponse = MemoryPackSerializer.Serialize(new BasicProtocolRes
                    {
                        Result = errorCode
                    });

                    // await using (var outStream = context.Response.Body) {
                    //     await outStream.WriteAsync(errorResponse);
                    // }

                    var writer = context.Response.BodyWriter;

                    await writer.WriteAsync(errorResponse);
                    await writer.FlushAsync();

                    return;
                }

                //uid를 키로 하는 데이터 없을 때 -> 로그인 안돼있음
                (errorCode, var userInfo) = await _memoryDB.GetUserAsync(user_id);

                if (errorCode != ErrorCode.None)
                {
                    errorCode = ErrorCode.AuthTokenKeyNotFound;

                    var errorResponse = MemoryPackSerializer.Serialize(new BasicProtocolRes
                    {
                        Result = errorCode
                    });

                    // await using (var outStream = context.Response.Body) {
                    //     await outStream.WriteAsync(errorResponse);
                    // }

                    var writer = context.Response.BodyWriter;

                    await writer.WriteAsync(errorResponse);
                    await writer.FlushAsync();

                    return;
                }

                //토큰이 일치하지 않을 때 -> 로그인 만료 혹은 이상한 유저
                errorCode = _loginAuthenticationService.IsInvalidUserAuthTokenThenSendError(userInfo, token);

                if (errorCode != ErrorCode.None)
                {
                    var errorResponse = MemoryPackSerializer.Serialize(new BasicProtocolRes
                    {
                        Result = errorCode
                    });

                    // await using (var outStream = context.Response.Body) {
                    //     await outStream.WriteAsync(errorResponse);
                    // }

                    var writer = context.Response.BodyWriter;

                    await writer.WriteAsync(errorResponse);
                    await writer.FlushAsync();

                    return;
                }


                // HttpContext를 컨트롤러로 보내주기
                context.Items[nameof(DaoMdbUserData)] = userInfo;
                context.Items[nameof(AuthProtocolReq)] = authReq;
                context.Items["AESKey"] = key;

                // Call the next delegate/middleware in the pipeline
                await _next(context);

            } catch (Exception ex) {

                _logger.LogError($"Error in Auth Method {ex.ToString()}");

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
}
