

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
    public partial class DaoMasterDBMailContent {
        
        public int mail_id { get; set; }
        public string mail_title { get; set; }
        public string mail_content { get; set; }

    }

    public class DaoMasterDBMailContentCacheMaker : CacheMaker {
        public DaoMasterDBMailContentCacheMaker(string key) : base(key) {
        }

        public override async Task<bool> MakeCacheData(QueryFactory _queryFactory, IMemoryDB_Test _memoryDB, IDbTransaction transaction = null) {

            var datas = (await _queryFactory
                        .Query("master_mail_content")
                        .GetAsync<DaoMasterDBMailContent>()).ToList();


            if ((await _memoryDB.CacheMasterData(key, datas)) != ErrorCode.None) return false;

            return true;

        }
    }


}
