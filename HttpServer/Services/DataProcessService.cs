using ADHNetworkShared.Crypto;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.Repository;
using MemoryPack;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MemoryPack.Compression;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.IO;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Utilities;
using HttpServer.Repository.Interface;

namespace HttpServer.Services {
    public class DataProcessService : IDataProcessService {

        private readonly IOptions<KeyOption> _keyOption;

        public DataProcessService(IMemoryDB_Test memoryDB, IOptions<KeyOption> keyOption) {
            _keyOption = keyOption;
        }

        public (T, ECPublicKeyParameters) GetDecryptedAndDeserializedNoneAuthData<T>(HttpContext context) where T : ProtocolReq {
            
            return (context.Items[nameof(ProtocolReq)] as T, context.Items["SenderPublicKey"] as ECPublicKeyParameters);

        }

        public (T, byte[]) GetDecryptedAndDeserializedData<T>(HttpContext context) where T : AuthProtocolReq {

            return (context.Items[nameof(AuthProtocolReq)] as T, context.Items["AESKey"] as byte[]);

        }

        public async Task SendEecryptedAndSerializedData(ProtocolRes res, HttpResponse response, byte[] key, bool isCompress = false) {

            byte[] data = null;

            if (isCompress) {
                
                using var compressor = new BrotliCompressor();
                
                MemoryPackSerializer.Serialize(compressor, res);
                
                data = compressor.ToArray();

                response.Headers.Append("IsCompress", "true");
            } else {

                data = MemoryPackSerializer.Serialize(res);

                response.Headers.Append("IsCompress", "false");
            }

            (var d, var iv) = AES.EncryptAES(data, key);
            EncryptedData encryptedRes = new EncryptedData(d, iv);
            byte[] bytes = MemoryPackSerializer.Serialize(encryptedRes);

            response.Headers.Append("Content-Length", $"{bytes.Length}");
            
            // await using (Stream st = response.Body) {
            //         
            //     await st.WriteAsync(bytes, 0, bytes.Length);
            //
            // }
            
            var writer = response.BodyWriter;
            
            await writer.WriteAsync(bytes);
            await writer.FlushAsync();

        }

        public async Task SendEecryptedAndSerializedNoneAuthData(ProtocolRes res, HttpResponse response, ECPublicKeyParameters clientPublicKey, bool isCompress = false) {

            byte[] data = null;

            if (isCompress) {

                using var compressor = new BrotliCompressor();
                MemoryPackSerializer.Serialize(compressor, res);
                data = compressor.ToArray();

                response.Headers.Append("IsCompress", "true");

            } else {

                data = MemoryPackSerializer.Serialize(res);

                response.Headers.Append("IsCompress", "false");

            }

            var d = ECIES.EncryptECIES(data, clientPublicKey, ECIES.GenerateECIESKeyPair());

            response.Headers.Append("Content-Length", $"{d.Length}");

            // await using (Stream st = response.Body) {
            //
            //     await st.WriteAsync(d, 0, d.Length);
            //
            // }

            var writer = response.BodyWriter;
            
            await writer.WriteAsync(d);
            await writer.FlushAsync();
            
        }
    }
}
