using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoItemReq : AuthProtocolReq {

        public ItemType itemType { get; set; }

        public DtoItemReq(string authToken, long userid, ItemType itemType) : base(authToken, userid) {
            protocolID = ProtocolID.Item;
            this.itemType = itemType;
        }

    }

    [MemoryPackable]
    public partial class DtoUserItemInfosRes : ProtocolRes {
        public List<(ItemInfo, int)> userItemInfos { get; set; }

    }

}
