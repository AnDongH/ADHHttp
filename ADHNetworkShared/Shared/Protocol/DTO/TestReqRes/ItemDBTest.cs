using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class ItemDBTestReq : AuthProtocolReq {

        public bool isCache { get; set; }

        public ItemDBTestReq(string authToken, long userid, bool isCache) : base(authToken, userid) {

            protocolID = ProtocolID.ItemDBTest;
            this.isCache = isCache;
        }
   
    }

    [MemoryPackable]
    public partial class ItemDBUpdateTestReq : AuthProtocolReq {
        
        public List<(int, int)> updateInfo { get; set; }
        
        public ItemDBUpdateTestReq(string authToken, long userid) : base(authToken, userid) {

            protocolID = ProtocolID.ItemDBUpdateTest;

            updateInfo = new List<(int, int)>() { 
            
                (1,Random.Shared.Next(-3, 4)), 
                (2,Random.Shared.Next(-3, 4)), 
                (3,Random.Shared.Next(-3, 4)), 
                (4,Random.Shared.Next(-3, 4)), 
                (5,Random.Shared.Next(-3, 4)), 
                (6,Random.Shared.Next(-3, 4)), 
                (7,Random.Shared.Next(-3, 4)), 
                (8,Random.Shared.Next(-3, 4)), 
                (9,Random.Shared.Next(-3, 4)), 
                (10,Random.Shared.Next(-3, 4))
           
            };

        }
    
    }


    [MemoryPackable]
    public partial class ItemDBTestRes : ProtocolRes {

        public List<ItemInfo> infos { get; set; }
    
    }


}
