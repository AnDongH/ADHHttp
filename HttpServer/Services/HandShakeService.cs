using HttpServer.Services.Interface;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Crypto.Parameters;
using System.IO;
using System;
using System.Threading.Tasks;
using ADHNetworkShared.Crypto;
using ADHNetworkShared.Protocol.DTO;
using MemoryPack;
using HttpServer.Repository;
using Microsoft.Extensions.Options;
using ADHNetworkShared.Shared.Util;
using ADHNetworkShared.Protocol;
using HttpServer.Repository.Interface;

namespace HttpServer.Services {
    public class HandShakeService : IHandShakeService {

        private readonly IOptions<KeyOption> _keyOption;

        public HandShakeService(IOptions<KeyOption> keyOption) { 
            _keyOption = keyOption;
        }

        public async Task SendCommonPublicKey(HttpContext context) {

            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            using (var inStream = request.Body)
            using (var stream = new MemoryStream()) {

                await inStream.CopyToAsync(stream);
                stream.Position = 0;
                byte[] buffer = stream.ToArray();

                DtoHandShakeReq req = MemoryPackSerializer.Deserialize<ProtocolReq>(buffer) as DtoHandShakeReq;

                if (req.Version == ConfigData._config["Version"]) {
                    ProtocolRes res = new DtoHandShakeRes(Convert.FromBase64String(_keyOption.Value.PublicKey), true);
                    res.Result = ErrorCode.None;

                    await using (Stream outStream = response.Body) {

                        await outStream.WriteAsync(MemoryPackSerializer.Serialize(res));

                    }

                } else {

                    ProtocolRes res = new DtoHandShakeRes(null, false);
                    res.Result = ErrorCode.InValidVersion;

                    await using (Stream outStream = response.Body) {

                        await outStream.WriteAsync(MemoryPackSerializer.Serialize(res));

                    }

                }

            }

            // 응답 작

        }

    }

    public class KeyOption {

        public const string CommonKey = "CommonKey";

        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }

}
