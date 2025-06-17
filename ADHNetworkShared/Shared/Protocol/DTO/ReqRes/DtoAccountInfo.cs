using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoAccountInfoReq : AuthProtocolReq {

        public string nick_name { get; set; }
        public DtoAccountInfoReq(string authToken, long userid, string nick_name) : base(authToken, userid) {

            protocolID = ProtocolID.UserInfo;
            this.nick_name = nick_name;

        }
    
    }

}
