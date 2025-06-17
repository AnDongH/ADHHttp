using ADHNetworkShared.Protocol.DTO;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Crypto.Parameters;
using System.Threading.Tasks;

namespace HttpServer.Services {
    public interface IDataProcessService {

        public (T, byte[]) GetDecryptedAndDeserializedData<T>(HttpContext context) where T : AuthProtocolReq;
        public (T, ECPublicKeyParameters) GetDecryptedAndDeserializedNoneAuthData<T>(HttpContext context) where T : ProtocolReq;
        Task SendEecryptedAndSerializedData(ProtocolRes res, HttpResponse response, byte[] key, bool isCompress = false);
        Task SendEecryptedAndSerializedNoneAuthData(ProtocolRes res, HttpResponse response, ECPublicKeyParameters clientPublicKey, bool isCompress = false);

    }
}
