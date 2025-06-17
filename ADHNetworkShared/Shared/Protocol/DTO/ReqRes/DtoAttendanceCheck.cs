using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoAttendanceCheckReq : AuthProtocolReq {
        public DtoAttendanceCheckReq(string authToken, long userid) : base(authToken, userid) {

            protocolID = ProtocolID.CheckAttendance;

        }
        
    }

    [MemoryPackable]
    public partial class DtoAttendanceCheckRes : ProtocolRes {

        public List<int> receivableDays { get; set; }
        public List<AttendanceRewardInfo> allReward { get; set; }


    }

}
