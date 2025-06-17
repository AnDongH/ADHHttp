using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpServer.DAO {

    [MemoryPackable]
    public partial class DaoMdbUserData {
        public string AuthToken { get; set; }
        public long UId { get; set; }
    }

}
