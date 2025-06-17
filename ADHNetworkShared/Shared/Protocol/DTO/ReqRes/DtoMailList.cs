using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoMailListReq : AuthProtocolReq {
    
        public DtoMailListReq(string authToken, long userid) : base(authToken, userid) {

            protocolID = ProtocolID.Mail;

        }
    
    }

    [MemoryPackable]
    public partial class DtoMailListRes : ProtocolRes {

        public List<MailInfo> mailInfos { get; set; }
        
    }

}
