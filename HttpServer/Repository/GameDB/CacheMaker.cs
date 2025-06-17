using ADHNetworkShared.Protocol;
using HttpServer.DAO;
using HttpServer.Repository.Interface;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HttpServer.Repository {

    public abstract class CacheMaker {

        public string key { get; set; }

        public CacheMaker(string key) {
            this.key = key;
        }

        public abstract Task<bool> MakeCacheData(QueryFactory _queryFactory, IMemoryDB_Test _memoryDB, IDbTransaction transaction = null);

    }

}
