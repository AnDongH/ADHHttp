using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoScoreSetReq : AuthProtocolReq {

        public int score { get; set; }

        public DtoScoreSetReq(string authToken, long userid, int score) : base(authToken, userid) {
            protocolID = ProtocolID.SetScore;
            this.score = score;
        }
    
    }

    [MemoryPackable]
    public partial class DtoGetMyRankingReq : AuthProtocolReq {
        public DtoGetMyRankingReq(string authToken, long userid) : base(authToken, userid) {
            protocolID = ProtocolID.GetMyRanking;
        }
    
    }

    [MemoryPackable]
    public partial class DtoGetAllRankingReq : AuthProtocolReq {
        public DtoGetAllRankingReq(string authToken, long userid) : base(authToken, userid) {
            protocolID= ProtocolID.GetAllRanking;
        }
    
    }

    [MemoryPackable]
    public partial class DtoGetMyRankingRes : ProtocolRes {

        public RankData MyRanking { get; set; }

    }

    [MemoryPackable]
    public partial class DtoGetAllRankingRes : ProtocolRes {

        public List<RankData> AllRankings { get; set; }

    }

}
