using ADHNetworkShared.Protocol;
using HttpServer.Repository;
using HttpServer.Repository.Interface;
using MemoryPack;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HttpServer.DAO {

    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class DaoMasterDBReward {

        public int item_id { get; set; }
        public int item_cnt { get; set; }

    }

    [MemoryPackable]
    public partial class DaoMasterDBAttendanceReward : DaoMasterDBReward {

        public int day_seq { get; set; }

    }

    [MemoryPackable]
    public partial class DaoMasterDBMailReward : DaoMasterDBReward {

        public int mail_id { get; set; }


    }

    public class DaoMasterDBAttendanceRewardCacheMaker : CacheMaker {

        public DaoMasterDBAttendanceRewardCacheMaker(string key) : base(key) {
        }

        public override async Task<bool> MakeCacheData(QueryFactory _queryFactory, IMemoryDB_Test _memoryDB, IDbTransaction transaction = null) {

            var datas = (await _queryFactory
                        .Query("master_attendance_reward")
                        .GetAsync<DaoMasterDBAttendanceReward>()).ToList();

            if ((await _memoryDB.CacheMasterData(key, datas)) != ErrorCode.None) return false;

            return true;

        }
    }

    public class DaoMasterDBMailRewardCacheMaker : CacheMaker {

        public DaoMasterDBMailRewardCacheMaker(string key) : base(key) {
        
        }

        public override async Task<bool> MakeCacheData(QueryFactory _queryFactory, IMemoryDB_Test _memoryDB, IDbTransaction transaction = null) {

            var datas = (await _queryFactory
                        .Query("master_mail_reward")
                        .GetAsync<DaoMasterDBMailReward>()).ToList();

            if ((await _memoryDB.CacheMasterData(key, datas)) != ErrorCode.None) return false;

            return true;

        }
    }

}
