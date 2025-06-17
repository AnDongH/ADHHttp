using ADHNetworkShared.Protocol.DTO;
using System.Threading.Tasks;

namespace HttpServer.Services {
    public interface IPingService {

        Task<BasicProtocolRes> UpdateTokenLife(PingReq request);

    }

}
