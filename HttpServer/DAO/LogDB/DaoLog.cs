using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpServer.DAO {

    public class DaoLog {

        public long uid { get; set; }
        public string logType { get; set; }
        public DateTime utcTime { get; set; }

        public DaoLog(long uid, string logType, DateTime utcTime) { 
        
            this.uid = uid;
            this.logType = logType;
            this.utcTime = utcTime;

        }
    }

    public class DaoRewardLog : DaoLog {

        public int reward_seq { get; set; }

        public DaoRewardLog(long uid,  int reward_seq, string logType, DateTime utcTime) : base(uid, logType, utcTime) {
    
            this.reward_seq = reward_seq;

        }

    }

    public class DaoFriendLog : DaoLog {
    
        public enum Req_Type {
            request,
            accept,
            delete,
            cancel,
            deny
        }


        public long friend_uid { get; set; }
        public string req_type { get; set; }
        
        public DaoFriendLog(long uid, long friend_uid, Req_Type req_type, string logType, DateTime utcTime) : base(uid, logType, utcTime) {
            
            this.friend_uid = friend_uid;
            this.req_type = req_type.ToString();
            
        }
    
    }

    public class DaoScoreLog : DaoLog {
        
        public int score { get; set; }
        
        public DaoScoreLog(long uid, int score, string logType, DateTime utcTime) : base(uid, logType, utcTime) {
            this.score = score;
        }
    
    }


}
