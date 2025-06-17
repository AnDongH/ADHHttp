using ADHNetworkShared.Protocol.DTO;
using System.Threading.Tasks;

namespace HttpServer.Services {

    public interface IItemService {

        Task<DtoUserItemInfosRes> GetItemList(DtoItemReq request);
        Task<DtoRewardRes> GetGatchaReward(DtoGatchaewardReq request);
    }

}
