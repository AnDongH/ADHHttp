using HttpServer.DAO;
using SqlKata.Execution;
using System.Data;
using System.Threading.Tasks;
using System;
using ADHNetworkShared.Protocol.DTO;
using System.Collections.Generic;
using ADHNetworkShared.Protocol;
using ZLogger;
using System.Linq;

namespace HttpServer.Repository {
    public partial class GameDB : IGameDB {

        public async Task<ErrorCode> UpdateUserItem(string table, IDbTransaction transaction, long uid, int item_id, int cnt) {

            var data = await _queryFactory
                             .Query(table)
                             .Where("uid", uid)
                             .Where("item_id", item_id)
                             .FirstOrDefaultAsync<DaoUserItem>(transaction);

            int affected = 0;

            if (data == null) {

                affected = await _queryFactory
                                 .Query(table)
                                 .InsertAsync(new {
                                     uid = uid,
                                     item_id = item_id,
                                     item_cnt = cnt
                                 }, transaction);

                if (affected == 0) {
                    transaction?.Rollback();
                    return ErrorCode.SQLAffectedZero;
                }

            } else {

                if ((data.cnt + cnt) <= 0) {
                    cnt = 0;
                } else {
                    cnt += data.cnt;
                }

                affected = await _queryFactory
                     .Query(table)
                     .Where("uid", uid)
                     .Where("item_id", item_id)
                     .UpdateAsync(new {
                         item_cnt = cnt
                     }, transaction);

                if (affected == 0) {
                    transaction?.Rollback();
                    return ErrorCode.SQLAffectedZero;
                }

            }

            return ErrorCode.None;

        }

        public async Task<(ErrorCode, List<(ItemInfo, int)>)> GetUserItemList(string table ,long uid) {

            List<(ItemInfo, int)> userItemInfos = new List<(ItemInfo, int)>();

            var daoUserItem = (await _queryFactory
                                    .Query(table)
                                    .Where("uid", uid)
                                    .GetAsync()).ToList();

            foreach (var data in daoUserItem) {

                var itemInfo = await GetMasterItemInfo(data.item_id);

                userItemInfos.Add((itemInfo, data.item_cnt));

            }

            return (ErrorCode.None, userItemInfos);

        }

        public async Task<ErrorCode> UpdateGatchaRewardInfo(GatchaRewardInfo rewardInfo, long uid) {

            using (var transaction = _dbConn.BeginTransaction()) {

                foreach (var item_info in rewardInfo.rewardInfo) {

                    string table = Router.UserItemTableMap[(ItemType)item_info.Item1.item_type_id];
                    ErrorCode code = await UpdateUserItem(table, transaction, uid, item_info.Item1.item_id, item_info.Item2);
                    
                    if (code != ErrorCode.None) {
                        transaction.Rollback();
                        return code;
                    }

                }

                transaction.Commit();

            }

            return ErrorCode.None;
        
        }


    }

}
