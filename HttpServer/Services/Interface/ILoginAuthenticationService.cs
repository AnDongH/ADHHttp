using ADHNetworkShared.Crypto;
using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.DAO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HttpServer.Services {
    public interface ILoginAuthenticationService {

        (ErrorCode, string) IsTokenNotExistOrReturnToken(AuthProtocolReq request);
        (ErrorCode, long) IsUserIDNotExistOrReturnUserID(AuthProtocolReq request);
        ErrorCode IsInvalidUserAuthTokenThenSendError(DaoMdbUserData userInfo, string token);

    }
}
