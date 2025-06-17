using ADHNetworkShared.Protocol.DTO;
using ADHNetworkShared.Protocol;
using HttpServer.DAO;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ZLogger;
using System.Linq;

namespace HttpServer.Repository {
    public partial class GameDB : IGameDB {

        private Dictionary<string, CacheMaker> cacheMakerMap = new Dictionary<string, CacheMaker>() {

            { "master_mail_content", new DaoMasterDBMailContentCacheMaker("master_mail_content")},
            { "master_mail_reward", new DaoMasterDBMailRewardCacheMaker("master_mail_reward")},
            { "master_attendance_reward", new DaoMasterDBAttendanceRewardCacheMaker("master_attendance_reward")},
            { "master_item", new DaoMasterDBItemCacheMaker("master_item")},
            { "master_gatcha_prob", new DaoMasterDBGatchaCacheMaker("master_gatcha_prob")},
            { "master_gatcha_reward", new DaoMasterDBGatchaRewardCacheMaker("master_gatcha_reward")},

        };

        public async Task<bool> LoadMasterData() {

            if (ActivateConnection()) {

                try {

                    bool flag = false;

                    foreach (var cache in cacheMakerMap) {
                        flag = await cache.Value.MakeCacheData(_queryFactory, _memoryDB);
                        if (!flag) return false;
                    }

                    return true;

                } catch (Exception ex) {

                    Console.WriteLine(ex.ToString());
                    _logger.ZLogError($"Error in get masterDB datas: {ex.ToString()}");
                    return false;
                }

            } else {

                return false;
            }

        }

        public async Task<ItemInfo> GetMasterItemInfo(int item_id) {

            var datas = await GetMasterDB<DaoMasterDBItem>("master_item");

            DaoMasterDBItem item = datas.Where(x => x.item_id == item_id).FirstOrDefault();

            if (item == null) return null;

            return item.GetItemInfo();


        }

        public async Task<List<ItemInfo>> GetMasterItemInfos(bool isCache) {
            IEnumerable<DaoMasterDBItem> datas = isCache
                ? await GetMasterDB<DaoMasterDBItem>("master_item")
                : await _queryFactory.Query("master_item").GetAsync<DaoMasterDBItemOther>();

            return datas.Select(data => data.GetItemInfo()).ToList();
        }


        public async Task<(ErrorCode, AttendanceRewardInfo)> GetAttendanceReward(int day) {

            var _daoMasterDBAttendanceRewards = await GetMasterDB<DaoMasterDBAttendanceReward>("master_attendance_reward");

            var rewards = _daoMasterDBAttendanceRewards
                          .Where(att => att.day_seq == day)
                          .Select(async att => (await GetMasterItemInfo(att.item_id), att.item_cnt));

            if (rewards == null) return (ErrorCode.SQLFailException, null);

            return (ErrorCode.None, new AttendanceRewardInfo(day, (await Task.WhenAll(rewards)).ToList()));

        }

        public async Task<(ErrorCode, List<AttendanceRewardInfo>)> GetAllAttendanceReward() {

            var _daoMasterDBAttendanceRewards = await GetMasterDB<DaoMasterDBAttendanceReward>("master_attendance_reward");

            var rewards = _daoMasterDBAttendanceRewards
                          .GroupBy(att => att.day_seq)
                          .Select(async g => new AttendanceRewardInfo(g.Key, (await Task.WhenAll(g.Select(async att => (await GetMasterItemInfo(att.item_id), att.item_cnt)))).ToList()));


            if (rewards == null) return (ErrorCode.SQLFailException, null);

            return (ErrorCode.None, (await Task.WhenAll(rewards)).ToList());

        }

        public async Task<(ErrorCode, MailRewardInfo)> GetMailReward(int mail_id) {

            var _daoMasterDBMailRewards = await GetMasterDB<DaoMasterDBMailReward>("master_mail_reward");

            var rewards = _daoMasterDBMailRewards
                             .Where(att => att.mail_id == mail_id)
                             .Select(async att => (await GetMasterItemInfo(att.item_id), att.item_cnt));

            if (rewards == null) return (ErrorCode.SQLFailException, null);

            return (ErrorCode.None, new MailRewardInfo(mail_id, (await Task.WhenAll(rewards)).ToList()));

        }

        public async Task<(ErrorCode, DaoMasterDBMailContent)> GetMailContent(int mail_id) {

            var _daoMasterDBMailContents = await GetMasterDB<DaoMasterDBMailContent>("master_mail_content");

            DaoMasterDBMailContent content = _daoMasterDBMailContents
                                             .Where(x => x.mail_id == mail_id)
                                             .FirstOrDefault();

            if (content != null) {
                return (ErrorCode.None, content);
            }

            return (ErrorCode.SQLFailException, null);

        }



        // 음.....
        public async Task<List<TCache>> GetMasterDB<TCache>(string key) {

            CacheMaker cacheMaker = cacheMakerMap[key];

            ErrorCode code = ErrorCode.None;
            List<TCache> list = null;

            (code, list) = await _memoryDB.GetCashedMasterData<TCache>(cacheMaker.key);

            if (code != ErrorCode.None || list == null) {

                await cacheMaker.MakeCacheData(_queryFactory, _memoryDB);

                (code, list) = await _memoryDB.GetCashedMasterData<TCache>(cacheMaker.key);

            }

            return list;

        }

    }

}
