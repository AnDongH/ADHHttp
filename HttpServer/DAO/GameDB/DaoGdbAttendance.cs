using System;
using System.Collections.Generic;
using System.Text;

namespace HttpServer.DAO {
    public class DaoGdbAttendanceInfo {
        public long uid { get; set; }
        public int attendance_cnt { get; set; }
        public DateTime recent_attendance_dt { get; set; }
    }

    public class DaoGdbAttendanceReward {
        public bool is_received { get; set; }

        public DaoGdbAttendanceReward(bool is_received) {

            this.is_received = is_received;

        }

    }

}
