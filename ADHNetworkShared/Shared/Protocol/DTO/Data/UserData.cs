using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class UserData {
        public long uid { get; set; }
        public string nick_name { get; set; }

        public UserData(long uid, string nick_name) {
            this.uid = uid;
            this.nick_name = nick_name;
        }

    }

    [MemoryPackable]
    public partial class RankData : UserData {
        
        public int score { get; set; }
        public long ranking { get; set; }
        public RankData(long uid, string nick_name, int score, long ranking) : base(uid, nick_name) {
        
            this.score = score;
            this.ranking = ranking;

        }
    
    }

}
