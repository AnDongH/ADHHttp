using MemoryPack;
using System;


namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class ProtocolRes {
        
        public ErrorCode Result { get; set; }

        public override string ToString() {
            return $"[ErrorCode]: {Result}";
        }

    }

}
