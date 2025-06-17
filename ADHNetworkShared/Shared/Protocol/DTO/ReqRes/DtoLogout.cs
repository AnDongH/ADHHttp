using MemoryPack;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoLogoutReq : AuthProtocolReq {

        public DtoLogoutReq(string authToken, long userid) : base(authToken, userid) {

            protocolID = ProtocolID.Logout;

        }


    }

}
