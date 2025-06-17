using System.Threading.Tasks;
using HttpServer.Controllers;
using ADHNetworkShared.Protocol.DTO;
using ADHNetworkShared.Protocol;
using HttpServer.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;
using ADHNetworkShared.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using HttpServer.Repository.Interface;


namespace HttpServer.Services {

    public class LoginService : ILoginService {

        private readonly IGameDB _gameDB;
        private readonly IMemoryDB_Test _memoryDB;
        private readonly ILogger<LoginService> _logger;

        public LoginService(ILogger<LoginService> logger, IGameDB gameDB, IMemoryDB_Test memoryDB) {
            _gameDB = gameDB;
            _memoryDB = memoryDB;
            _logger = logger;
        }

        public async Task<DtoLoginRes> GetLoginResponse(DtoLoginReq request) {
            var response = new DtoLoginRes();

            if (_gameDB.ActivateConnection()) {

                (ErrorCode errorCode, long uid) = await _gameDB.AuthCheck(request.ID, request.PW);

                if (errorCode != ErrorCode.None) {
                    response.Result = errorCode;
                    return response;
                }


                string authToken = Security.CreateAuthToken();
                errorCode = await _memoryDB.RegistUserAsync(authToken, uid);
                if (errorCode != ErrorCode.None) {
                    response.Result = errorCode;
                    return response;
                }

                AsymmetricCipherKeyPair pair = ECDH.GenerateECKeyPair();
                var pub = pair.Public as ECPublicKeyParameters;
                var ppk = pair.Private as ECPrivateKeyParameters;

                byte[] aesKey = ECDH.GenerateSharedSecret(ppk, ECDH.RestorePublicBytesToKey(request.ClientPublicKey));

                errorCode = await _memoryDB.RegistClientKeyAsync(uid, aesKey);

                if (errorCode != ErrorCode.None) {
                    response.Result = errorCode;
                    return response;
                }

                (errorCode, var rawData) = await _gameDB.GetUserInfo(uid);

                if (errorCode != ErrorCode.None) {
                    response.Result = errorCode;
                    return response;
                }

                response.userName = rawData.nick_name;
                response.AuthToken = authToken;
                response.Uid = uid;
                response.ServerPublicKey = pub.Q.GetEncoded();

            } else {

                response.Result = ErrorCode.DBConnectionFailException;

            }

             

            return response;
        }

        public async Task<BasicProtocolRes> GetLogoutResponse(DtoLogoutReq request) {
            
            var response = new BasicProtocolRes();

            response.Result = await _memoryDB.DeleteUserAsync(request.UserID);
            response.Result = await _memoryDB.DeleteClientKeyAsync(request.UserID);

            return response;
        }
    }
}
