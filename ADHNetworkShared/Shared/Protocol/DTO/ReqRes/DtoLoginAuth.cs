using MemoryPack;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoLoginAuthReq : AuthProtocolReq {

        public DtoLoginAuthReq(string authToken,long userid) : base(authToken, userid) {

            protocolID = ProtocolID.LoginAuth;

        }
    }

}
