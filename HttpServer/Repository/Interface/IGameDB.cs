using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.DAO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace HttpServer.Repository {
    public interface IGameDB : IDisposable {

        bool ActivateConnection();

        #region account

        Task<(ErrorCode, long)> AuthCheck(string email, string pw);
        Task<ErrorCode> PWCheck(long uid, string pw);
        Task<ErrorCode> CreateUserAccount(string email, string pw);
        Task<ErrorCode> DeleteUserAccount(long uid);
        Task<(ErrorCode, DaoGdbUserInfo)> GetUserInfo(long uid);
        Task<ErrorCode> SetUserInfo(long uid, string nick_name);

        #endregion


        #region attendance

        Task<(ErrorCode, DaoGdbAttendanceInfo)> GetAttendance(long uid);
        Task<ErrorCode> SetAttendance(long uid);
        Task<(ErrorCode, List<int>)> CheckAttendanceReward(DaoGdbAttendanceInfo info, long uid);
        Task<ErrorCode> UpdateAttendanceRewardInfo(int day, long uid);

        #endregion


        #region mail system

        Task<ErrorCode> DeleteMail(long uid, int mail_id);
        Task<(ErrorCode, List<MailInfo>)> GetMailList(long uid);
        Task<(ErrorCode, MailRewardInfo)> UpdateMailRewardInfo(int mail_id, long uid);
        Task<ErrorCode> CheckMailRewardReceived(long uid, int mail_id);

        #endregion

        #region item system
        Task<(ErrorCode, List<(ItemInfo, int)>)> GetUserItemList(string table, long uid);
        Task<ErrorCode> UpdateUserItem(string table, IDbTransaction transaction, long uid, int item_id, int cnt);
        Task<ErrorCode> UpdateGatchaRewardInfo(GatchaRewardInfo rewardInfo, long uid);

        #endregion

        #region friend system

        Task<(ErrorCode, List<DaoGdbFriendInfo>)> GetFriendInfoList(long uid);
        Task<(ErrorCode, List<DaoGdbFriendInfo>)> GetFriendReqInfo(long uid);
        Task<(ErrorCode, List<DaoGdbFriendInfo>)> GetFriendReceivedInfo(long uid);
        Task<ErrorCode> InsertFriendReq(long uid, long friendUid);
        Task<ErrorCode> InsertFriendReq(long uid, long friendUid, IDbTransaction transaction);
        Task<ErrorCode> AcceptFriendReq(long uid, long friendUid);
        Task<ErrorCode> DeleteFriend(long uid, long friendUid); 
        Task<ErrorCode> CancelOrDenyFriendReq(long uid, long friendUid);

        #endregion

        #region ranking
        
        Task<ErrorCode> SetScore(long uid, int score);
        Task<(ErrorCode, DaoGdbUserScore)> GetMyRanking(long uid);
        Task<(ErrorCode, List<DaoGdbUserScore>)> GetAllRankings();

        #endregion

        #region master
        Task<(ErrorCode, AttendanceRewardInfo)> GetAttendanceReward(int day);
        Task<(ErrorCode, List<AttendanceRewardInfo>)> GetAllAttendanceReward();
        Task<(ErrorCode, MailRewardInfo)> GetMailReward(int mail_id);
        Task<ItemInfo> GetMasterItemInfo(int item_id);
        Task<bool> LoadMasterData();
        Task<List<TCache>> GetMasterDB<TCache>(string key);
        Task<(ErrorCode, DaoMasterDBMailContent)> GetMailContent(int mail_id);




        Task<List<ItemInfo>> GetMasterItemInfos(bool isCache);
        #endregion

    }

}
