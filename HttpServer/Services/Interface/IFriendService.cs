using ADHNetworkShared.Protocol.DTO;
using System.Threading.Tasks;

namespace HttpServer.Services {
    public interface IFriendService {

        Task<DtoFriendInfoListRes> GetFriendInfoList(DtoFriendInfoListReq request);
        Task<DtoFriendInfoListRes> GetFriendReqInfoList(DtoFriendReqInfoListReq request);
        Task<DtoFriendInfoListRes> GetFriendReceivedInfoList(DtoFriendReceivedInfoListReq request);
        Task<BasicProtocolRes> InsertFriendReq(DtoFriendReqReq request);
        Task<BasicProtocolRes> AcceptFriendReq(DtoFriendAcceptReq request);
        Task<BasicProtocolRes> DeleteFriend(DtoFriendDeleteReq request);
        Task<BasicProtocolRes> CancelFriendReq(DtoFriendReqCancelReq request);
        Task<BasicProtocolRes> DenyFriendReq(DtoFriendReqDenyReq request);
    
    }

}
