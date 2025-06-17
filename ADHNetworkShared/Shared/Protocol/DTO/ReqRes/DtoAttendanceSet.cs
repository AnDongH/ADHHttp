using ADHNetworkShared.Protocol.DTO;
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoAttendanceSetReq : AuthProtocolReq {

        public DtoAttendanceSetReq(string authToken, long userid) : base(authToken, userid) {

            protocolID = ProtocolID.SetAttendance;

        }

    }

}
