using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HttpServer.Services {
    public interface IMailService {
    
        Task<DtoMailListRes> GetMailList(DtoMailListReq request);
        Task<DtoRewardRes> ReceiveMailReward(DtoMailRewardReq request);
        Task<BasicProtocolRes> DeleteMail(DtoMailDeleteReq request);
    
    }

}
