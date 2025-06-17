using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.DAO;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ZLogger;

namespace HttpServer.Repository {
    public partial class GameDB : IGameDB {
    
        public async Task<ErrorCode> DeleteMail(long uid, int mail_id) {

            int affected = await _queryFactory.Query("user_mailbox")
                                              .Where("uid", uid)
                                              .Where("mail_id", mail_id)
                                              .DeleteAsync();

            if (affected == 0) return ErrorCode.SQLAffectedZero;

            return ErrorCode.None;

        }

        public async Task<(ErrorCode, List<MailInfo>)> GetMailList(long uid) {

            // gameDB 에서 uid에 맞는 정보들 가져오기
            var result = await _queryFactory
                               .Query("user_mailbox")
                               .Where("uid", uid)
                               .Select("mail_id", "expire_dt", "is_received")
                               .GetAsync<DaoGdbMail>();

            if (result == null) return (ErrorCode.SQLInfoDoesNotExist, null);

            List<MailInfo> mailInfos = new List<MailInfo>();

            ErrorCode errorCode = ErrorCode.None;

            // gameDB 정보로
            foreach (var daoMail in result) {

                // 메일 콘텐츠 masterDB에서 가져오기
                (errorCode, var daoMailContent) = await GetMailContent(daoMail.mail_id);

                if (errorCode != ErrorCode.None) return (errorCode, mailInfos);

                // 메일 보상 masterDB에서 가져오기
                (errorCode, var daoMailReward) = await GetMailReward(daoMail.mail_id);

                if (errorCode != ErrorCode.None) return (errorCode, mailInfos);

                TimeSpan dif = daoMail.expire_dt - DateTime.UtcNow;

                mailInfos.Add(new MailInfo(daoMailReward,
                                           daoMailContent.mail_title,
                                           daoMailContent.mail_content,
                                           daoMailContent.mail_id,
                                           dif.Days,
                                           daoMail.is_received));


            }

            return (ErrorCode.None, mailInfos);

        }

        public async Task<(ErrorCode, MailRewardInfo)> UpdateMailRewardInfo(int mail_id, long uid) {


            using (var transaction = _dbConn.BeginTransaction()) {

                int affected = await _queryFactory
                    .Query("user_mailbox")
                    .Where("uid", uid)
                    .Where("mail_id", mail_id)
                    .UpdateAsync(new { is_received = true }, transaction);


                if (affected == 0) {
                    transaction.Rollback();
                    return (ErrorCode.SQLFailException, null);
                }


                var masterMailReward = (await GetMasterDB<DaoMasterDBMailReward>("master_mail_reward"))
                                        .Where(m => m.mail_id == mail_id);

                var masteritemList = await GetMasterItemInfos(true);

                foreach (var data in masterMailReward) {


                    ItemType type = (ItemType)masteritemList.Find(x => x.item_id == data.item_id).item_type_id;
                    string table = Router.UserItemTableMap[type];
                    ErrorCode code = await UpdateUserItem(table, transaction, uid, data.item_id, data.item_cnt);

                    if (code != ErrorCode.None) {
                        transaction.Rollback();
                        return (code, null);
                    }

                }

                transaction.Commit();

                return await GetMailReward(mail_id);

            }


        }

        public async Task<ErrorCode> CheckMailRewardReceived(long uid, int mail_id) {

            var result = await _queryFactory
                                    .Query("user_mailbox")
                                    .Where("uid", uid)
                                    .Where("mail_id", mail_id)
                                    .Select("mail_id", "expire_dt", "is_received")
                                    .FirstOrDefaultAsync<DaoGdbMail>();

            if (result == null) return ErrorCode.SQLInfoDoesNotExist;
            else return result.is_received ? ErrorCode.AlreadyReceivedReward : ErrorCode.None;

        }


    }


}
