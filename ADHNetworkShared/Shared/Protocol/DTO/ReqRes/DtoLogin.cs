
using MemoryPack;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoLoginReq : ProtocolReq {

        public string ID { get; set; }
        public string PW { get; set; }
        public byte[] ClientPublicKey { get; set; }

        public DtoLoginReq(string id, string pw, byte[] clientPublicKey) {

            ID = id;
            PW = pw;
            ClientPublicKey = clientPublicKey;
            protocolID = ProtocolID.Login;

        }

    }

    [MemoryPackable]
    public partial class DtoLoginRes : ProtocolRes {
        public string AuthToken { get; set; }
        public long Uid { get; set; }
        public string userName { get; set; }
        public byte[] ServerPublicKey { get; set; }

    }

}
