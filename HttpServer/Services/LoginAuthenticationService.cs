using HttpServer.DAO;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ADHNetworkShared.Protocol.DTO;
using ADHNetworkShared.Protocol;
using ADHNetworkShared.Crypto;
using MemoryPack;
using HttpServer.Repository;
using HttpServer.Repository.Interface;

namespace HttpServer.Services {
    public class LoginAuthenticationService : ILoginAuthenticationService {

        private readonly IMemoryDB_Test _memoryDB;
        public LoginAuthenticationService(IMemoryDB_Test memoryDB) { 
        
            _memoryDB = memoryDB;

        }

        // authtoken이 헤더에 있는지 확인하고 있으면 token 리턴 없으면 오류 메시지
        public (ErrorCode, string) IsTokenNotExistOrReturnToken(AuthProtocolReq request) {

            return string.IsNullOrEmpty(request.AuthToken) ? (ErrorCode.AuthTokenNotExist, request.AuthToken) : (ErrorCode.None, request.AuthToken);

        }

        // userid가 헤더에 있는지 확인하고 있으면 id 리턴 없으면 오류메시지
        public (ErrorCode, long) IsUserIDNotExistOrReturnUserID(AuthProtocolReq request) {
            
            return request.UserID == 0 ? (ErrorCode.UserIDNotExist, request.UserID) : (ErrorCode.None, request.UserID);
        
        }

        // 토큰이 다름. 새로 로그인 하던가 해야함
        public ErrorCode IsInvalidUserAuthTokenThenSendError(DaoMdbUserData userInfo, string token) {
           
            return (string.CompareOrdinal(userInfo.AuthToken, token) == 0) ? ErrorCode.None : ErrorCode.InValidAuthToken;
        
        }

    }
}
