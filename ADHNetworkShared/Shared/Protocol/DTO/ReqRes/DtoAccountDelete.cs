using MemoryPack;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoAccountDeleteReq : AuthProtocolReq {
        public string PW { get; set; }

        public DtoAccountDeleteReq(string authToken, long userid, string pw) : base(authToken, userid) {

            PW = pw;
            protocolID = ProtocolID.DeleteAccount;
        
        }

    }

}
