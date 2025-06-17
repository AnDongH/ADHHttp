using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class PingReq : AuthProtocolReq {
        public PingReq(string authToken, long userid) : base(authToken, userid) {

            protocolID = ProtocolID.Ping;

        }
    
    }

}
