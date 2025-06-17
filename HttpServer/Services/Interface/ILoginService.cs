using System.Threading.Tasks;
using ADHNetworkShared.Protocol.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HttpServer.Services {
    public interface ILoginService {

        Task<DtoLoginRes> GetLoginResponse(DtoLoginReq request);
        Task<BasicProtocolRes> GetLogoutResponse(DtoLogoutReq request);
    }
}
