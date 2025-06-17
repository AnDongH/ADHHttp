using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class PostTestReq : ProtocolReq {
        public string requestMSG { get; set; }
        public PostTestReq(string requestMSG) {
            protocolID = ProtocolID.PostTest;
            this.requestMSG = requestMSG;
        }

    }

    [MemoryPackable]
    public partial class AuthPostTestReq : AuthProtocolReq {
        public string requestMSG { get; set; }
        public AuthPostTestReq(string requestMSG, string authToken, long userId) : base(authToken, userId) {
            protocolID = ProtocolID.AuthPostTest;
            this.requestMSG = requestMSG;
        }

    }

    [MemoryPackable]
    public partial class PostTestRes : ProtocolRes {
        public string responseMSG { get; set; }
    
        public PostTestRes(string responseMSG) : base() {
            this.responseMSG = responseMSG;
        }

    }
}
