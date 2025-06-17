using SqlKata.Execution;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;
using HttpServer.DAO;
using ZLogger;
using ADHNetworkShared.Protocol;
using System.Linq;
using System.Transactions;

namespace HttpServer.Repository
{
    public partial class GameDB : IGameDB
    {

        public async Task<(ErrorCode, List<DaoGdbFriendInfo>)> GetFriendInfoList(long uid) {

            var list = (await _queryFactory
                              .Query("user_friend")
                              .Where(q => q.Where("friend_uid", uid).Where("is_accepted", true))       // 나에게 온 요청중 수락한 것들
                              .OrWhere(q => q.Where("uid", uid).Where("is_accepted", true))            // 내가 보낸 요청 중 수락된 것들
                              .Select("uid", "friend_uid")
                              .GetAsync<DaoGdbFriendInfo>()).ToList();

            return list == null ? (ErrorCode.SQLInfoDoesNotExist, null) : (ErrorCode.None, list);

        }


        // uid가 friend_uid 에게 요청한 것 중 수락되지 않은 것만 가져옴
        public async Task<(ErrorCode, List<DaoGdbFriendInfo>)> GetFriendReqInfo(long uid) {

            var data = (await _queryFactory
                             .Query("user_friend")
                             .Where("uid", uid)
                             .Where("is_accepted", false)
                             .Select("uid", "friend_uid")
                             .GetAsync<DaoGdbFriendInfo>()).ToList();

            return data == null ? (ErrorCode.SQLInfoDoesNotExist, null) : (ErrorCode.None, data);

        }

        public async Task<(ErrorCode, List<DaoGdbFriendInfo>)> GetFriendReceivedInfo(long uid) {
            
            var data = (await _queryFactory
                             .Query("user_friend")
                             .Where("friend_uid", uid)
                             .Where("is_accepted", false)
                             .Select("uid", "friend_uid")
                             .GetAsync<DaoGdbFriendInfo>()).ToList();

            return data == null ? (ErrorCode.SQLInfoDoesNotExist, null) : (ErrorCode.None, data);

        }

        public async Task<ErrorCode> InsertFriendReq(long uid, long friendUid) {

            var data = await _queryFactory
                             .Query("user_friend")
                             .Where(q => q.Where("uid", uid).Where("friend_uid", friendUid))
                             .OrWhere(q => q.Where("uid", friendUid).Where("friend_uid", uid))
                             .Select("uid", "friend_uid")
                             .FirstOrDefaultAsync<DaoGdbFriendInfo>();

            if (data != null) return ErrorCode.AlreadyRequestedOrRegisteredFriend;

            int affected = await _queryFactory
                                 .Query("user_friend")
                                 .InsertAsync(new {
                                     uid = uid,
                                     friend_uid = friendUid,
                                     is_accepted = false,
                                     create_dt = DateTime.UtcNow,
                                 });

            return affected == 0 ? ErrorCode.SQLAffectedZero : ErrorCode.None;

        }

        public async Task<ErrorCode> InsertFriendReq(long uid, long friendUid, IDbTransaction transaction) {

            int affected = await _queryFactory
                                 .Query("user_friend")
                                 .InsertAsync(new {
                                     uid = uid,
                                     friend_uid = friendUid,
                                     is_accepted = false,
                                     create_dt = DateTime.UtcNow,
                                 }, transaction);

            return affected == 0 ? ErrorCode.SQLAffectedZero : ErrorCode.None;

        }

        public async Task<ErrorCode> AcceptFriendReq(long uid, long friendUid) {

            int affected = await _queryFactory
                                 .Query("user_friend")
                                 .Where("uid", friendUid)     // 나에게 온 요청을 수락
                                 .Where("friend_uid", uid)
                                 .UpdateAsync(new {
                                     is_accepted = true,
                                 });

            return affected == 0 ? ErrorCode.SQLAffectedZero : ErrorCode.None;

        }

        public async Task<ErrorCode> DeleteFriend(long uid, long friendUid) {

            int affected = await _queryFactory
                                 .Query("user_friend")
                                 .WhereRaw($"(uid=? AND friend_uid=?) OR (uid=? AND friend_uid=?)", uid, friendUid, friendUid, uid)
                                 .DeleteAsync();

            return affected == 0 ? ErrorCode.SQLAffectedZero : ErrorCode.None;

        }

        /// <summary>
        /// uid = uid, friendUid = friendUid => Cancel /
        /// uid = friendUid, friendUid = uid => Deny
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="friendUid"></param>
        /// <returns></returns>
        public async Task<ErrorCode> CancelOrDenyFriendReq(long uid, long friendUid) {

            int affected = await _queryFactory
                                 .Query("user_friend")
                                 .Where("uid", uid)                     // 내가
                                 .Where("friend_uid", friendUid)        // 상대에게 보낸 요청
                                 .DeleteAsync();                        // 취소

            return affected == 0 ? ErrorCode.SQLAffectedZero : ErrorCode.None;

        }


    }

}
