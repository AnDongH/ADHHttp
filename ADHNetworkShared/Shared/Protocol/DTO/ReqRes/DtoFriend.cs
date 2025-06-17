using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO
{

    // DB에 정보 집어넣기 uid => friend_id 로 요청
    [MemoryPackable]
    public partial class DtoFriendReqReq : AuthProtocolReq {
        
        public long friend_uid { get; set; }
        
        public DtoFriendReqReq(string authToken, long userid, long friend_uid) : base(authToken, userid) {

            protocolID = ProtocolID.FriendReq;
            this.friend_uid = friend_uid;

        }
    
    }

    // is_received를 true로 변경
    [MemoryPackable]
    public partial class DtoFriendAcceptReq : AuthProtocolReq {

        public long friend_uid { get; set; }

        public DtoFriendAcceptReq(string authToken, long userid, long friend_uid) : base(authToken, userid) {

            protocolID = ProtocolID.FriendAccept;
            this.friend_uid = friend_uid;
            
        }

    }

    // 현재 친구 모두 가져오기
    [MemoryPackable]
    public partial class DtoFriendInfoListReq : AuthProtocolReq {

        public DtoFriendInfoListReq(string authToken, long userid) : base(authToken, userid) {

            protocolID = ProtocolID.FriendInfo;

        }

    }

    // 내가 요청한 친구 모두 가져오기
    [MemoryPackable]
    public partial class DtoFriendReqInfoListReq : AuthProtocolReq {

        public DtoFriendReqInfoListReq(string authToken, long userid) : base(authToken, userid) {

            protocolID = ProtocolID.FriendReqInfo;

        }

    }

    // 내가 받은 친구 요청 모두 가져오기
    [MemoryPackable]
    public partial class DtoFriendReceivedInfoListReq : AuthProtocolReq {

        public DtoFriendReceivedInfoListReq(string authToken, long userid) : base(authToken, userid) {

            protocolID = ProtocolID.FriendRcvInfo;
        
        }

    }

    [MemoryPackable]
    public partial class DtoFriendDeleteReq : AuthProtocolReq {

        public long friend_uid { get; set; }

        public DtoFriendDeleteReq(string authToken, long userid, long friend_uid) : base(authToken, userid) {
        
            protocolID = ProtocolID.FriendDelete;
            this.friend_uid = friend_uid;
        
        }
    
    }

    [MemoryPackable]
    public partial class DtoFriendReqCancelReq : AuthProtocolReq {

        public long friend_uid { get; set; }

        public DtoFriendReqCancelReq(string authToken, long userid, long friend_uid) : base(authToken, userid) {

            protocolID = ProtocolID.FriendCancel;
            this.friend_uid = friend_uid;

        }
    
    }

    [MemoryPackable]
    public partial class DtoFriendReqDenyReq : AuthProtocolReq {

        public long friend_uid { get; set; }

        public DtoFriendReqDenyReq(string authToken, long userid, long friend_uid) : base(authToken, userid) {

            protocolID = ProtocolID.FriendDeny;
            this.friend_uid = friend_uid;

        }

    }

    [MemoryPackable]
    public partial class DtoFriendInfoListRes : ProtocolRes {
        public List<UserData> userDatas { get; set; }

    }

}
