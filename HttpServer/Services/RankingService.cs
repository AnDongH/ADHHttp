using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.DAO;
using HttpServer.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HttpServer.Services {
    public class RankingService : IRankingService {

        private readonly IGameDB _gameDB;

        public RankingService(IGameDB gameDB) {
            _gameDB = gameDB;
        }

        public async Task<BasicProtocolRes> SetScore(DtoScoreSetReq request) {

            var res = new BasicProtocolRes();

            if (_gameDB.ActivateConnection()) {

                res.Result = await _gameDB.SetScore(request.UserID, request.score);

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;

        }

        public async Task<DtoGetMyRankingRes> GetMyRanking(DtoGetMyRankingReq request) {

            var res = new DtoGetMyRankingRes();

            if (_gameDB.ActivateConnection()) {

                (res.Result, var rawRankingData) = await _gameDB.GetMyRanking(request.UserID);

                if (res.Result != ErrorCode.None) return null;

                (res.Result, var userInfo) = await _gameDB.GetUserInfo(request.UserID);

                if (res.Result != ErrorCode.None) return null;

                res.MyRanking = new RankData(request.UserID, userInfo.nick_name, rawRankingData.score, rawRankingData.row_num);

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;

        }

        public async Task<DtoGetAllRankingRes> GetAllRankings(DtoGetAllRankingReq request) {

            var res = new DtoGetAllRankingRes();

            if (_gameDB.ActivateConnection()) {

                (res.Result, var rawRankingDatas) = await _gameDB.GetAllRankings();

                if (res.Result != ErrorCode.None) return null;

                res.AllRankings = new List<RankData>();

                foreach (var rankData in rawRankingDatas) {

                    (res.Result, var userInfo) = await _gameDB.GetUserInfo(rankData.uid);

                    if (res.Result != ErrorCode.None) return null;

                    res.AllRankings.Add(new RankData(rankData.uid, userInfo.nick_name, rankData.score, rankData.row_num));
                }

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;
        
        }

    }

}
