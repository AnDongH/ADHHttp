using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoMailDeleteReq : AuthProtocolReq {
        
        public int mail_id { get; set; }
        
        public DtoMailDeleteReq(string authToken, long userid, int mail_id) : base(authToken, userid) {

            protocolID = ProtocolID.DeleteMail;
            this.mail_id = mail_id;

        }
    
    }

}
