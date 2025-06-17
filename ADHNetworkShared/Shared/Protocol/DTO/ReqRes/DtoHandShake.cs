using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoHandShakeReq : ProtocolReq {

        public string Version { get; set; }

        public DtoHandShakeReq() {

            protocolID = ProtocolID.Handshake;

        }

    }

    [MemoryPackable]
    public partial class DtoHandShakeRes : ProtocolRes {

        public bool VersionOK { get; set; }
        public byte[] ServerCommonKey { get; set; }

        public DtoHandShakeRes(byte[] serverCommonKey, bool versionOK) {

            ServerCommonKey = serverCommonKey;
            VersionOK = versionOK;

        }

    }

}
