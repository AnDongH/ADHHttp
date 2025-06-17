using ADHNetworkShared.Protocol.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HttpServer.Services {
    public interface IAccountService {

        Task<BasicProtocolRes> CreateUserAccount(DtoAccountRegisterReq request);
        Task<BasicProtocolRes> DeleteUserAccount(DtoAccountDeleteReq request);
        Task<BasicProtocolRes> SetUserInfo(DtoAccountInfoReq request);
    }
}
