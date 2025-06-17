using ADHNetworkShared.Protocol.DTO;
using System.Threading.Tasks;

namespace HttpServer.Services {
    public interface IRankingService {

        Task<BasicProtocolRes> SetScore(DtoScoreSetReq request);
        Task<DtoGetMyRankingRes> GetMyRanking(DtoGetMyRankingReq request);
        Task<DtoGetAllRankingRes> GetAllRankings(DtoGetAllRankingReq request);

    }

}
