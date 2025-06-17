using MemoryPack;
using System;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoAttendanceGetReq : AuthProtocolReq {

        public DtoAttendanceGetReq(string authToken ,long userid) : base(authToken, userid) {

            protocolID = ProtocolID.GetAttendance;

        }

    }

    [MemoryPackable]
    public partial class DtoAttendanceGetRes : ProtocolRes {
        public long uid { get; set; }
        public int attendance_cnt { get; set; }
        public DateTime recent_attendance_dt { get; set; }

    }

}
