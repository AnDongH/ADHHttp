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

    [MemoryPackable]
    public partial class DaoMasterDBGatchaProb {
        
        public int item_rarity_id { get; set; }
        public int item_prob { get; set; }
    
    }

    [MemoryPackable]
    public partial class DaoMasterDBGatchaReward {

        public int item_id { get; set; }
        public int item_cnt { get; set; }

    }

    public class DaoMasterDBGatchaCacheMaker : CacheMaker {
        public DaoMasterDBGatchaCacheMaker(string key) : base(key) {
     
        }

        public override async Task<bool> MakeCacheData(QueryFactory _queryFactory, IMemoryDB_Test _memoryDB, IDbTransaction transaction = null) {

            var datas = (await _queryFactory.Query("master_gatcha_prob").GetAsync<DaoMasterDBGatchaProb>()).ToList();

            if ((await _memoryDB.CacheMasterData(key, datas)) != ErrorCode.None) return false;

            return true;

        }

    }

    public class DaoMasterDBGatchaRewardCacheMaker : CacheMaker {
        public DaoMasterDBGatchaRewardCacheMaker(string key) : base(key) {

        }

        public override async Task<bool> MakeCacheData(QueryFactory _queryFactory, IMemoryDB_Test _memoryDB, IDbTransaction transaction = null) {

            var datas = (await _queryFactory.Query("master_gatcha_reward").GetAsync<DaoMasterDBGatchaReward>()).ToList();

            if ((await _memoryDB.CacheMasterData(key, datas)) != ErrorCode.None) return false;

            return true;

        }

    }

}
