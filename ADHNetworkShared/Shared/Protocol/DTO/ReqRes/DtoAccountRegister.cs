
using MemoryPack;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoAccountRegisterReq : ProtocolReq {

        public string ID { get; set; }
        public string PW { get; set; }
        public DtoAccountRegisterReq(string id, string pw) {

            ID = id;
            PW = pw;
            protocolID = ProtocolID.CreateAccount;
        }

    }

}
