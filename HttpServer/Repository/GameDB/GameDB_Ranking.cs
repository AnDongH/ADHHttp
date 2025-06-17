using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.DAO;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpServer.Repository {
    
    public partial class GameDB : IGameDB {
        
        public async Task<ErrorCode> SetScore(long uid, int score) {

            int affected = 0;

            var userScore = await _queryFactory
                                  .Query("user_score")
                                  .Where("uid", uid)
                                  .FirstOrDefaultAsync<DaoGdbUserScore>();

            if (userScore == null) {

                affected = await _queryFactory
                                 .Query("user_score")
                                 .Where("uid", uid)
                                 .InsertAsync(new { uid = uid, score = score});

                if (affected == 0) return ErrorCode.SQLAffectedZero;

            } else {

                affected = await _queryFactory
                                 .Query("user_score")
                                 .Where("uid", uid)
                                 .IncrementAsync("score", score);

                if (affected == 0) return ErrorCode.SQLAffectedZero;

            }

            return ErrorCode.None;

        }

        public async Task<(ErrorCode, DaoGdbUserScore)> GetMyRanking(long uid) {

            bool isExists = await _queryFactory
                                 .Query("user_score")
                                 .Where("uid", uid)
                                 .ExistsAsync();

            if (!isExists) {

                int affected = await _queryFactory
                                 .Query("user_score")
                                 .Where("uid", uid)
                                 .InsertAsync(new { uid = uid, score = 0 });

                if (affected == 0) return (ErrorCode.SQLAffectedZero, null);
           

            }


            var datas = (await _queryFactory
                              .Query("user_score")
                              .Select("uid", "score")
                              .SelectRaw("ROW_NUMBER() OVER (ORDER BY score DESC) AS row_num")
                              .GetAsync<DaoGdbUserScore>()).ToList();

            var data = datas.Where(x => x.uid == uid).FirstOrDefault();

            return data == null ? (ErrorCode.SQLInfoDoesNotExist, null) : (ErrorCode.None, data);

        }

        public async Task<(ErrorCode, List<DaoGdbUserScore>)> GetAllRankings() {

            var datas = (await _queryFactory
                              .Query("user_score")
                              .Select("uid", "score")
                              .SelectRaw("ROW_NUMBER() OVER (ORDER BY score DESC) AS row_num")
                              .GetAsync<DaoGdbUserScore>()).ToList();

            return datas == null ? (ErrorCode.SQLInfoDoesNotExist, null) : (ErrorCode.None, datas);

        }

    }

}
