using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Crypto {

    [MemoryPackable]
    public partial class EncryptedData {

        public byte[] Data { get; private set; }
        public byte[] IV { get; private set; }

        public EncryptedData(byte[] data, byte[] iv) {
            Data = data;
            IV = iv;
        }

    }

    [MemoryPackable]
    public partial class ClientEncryptedData : EncryptedData {

        public long RedisKey { get; private set; }

        public ClientEncryptedData(byte[] data, byte[] iv, long redisKey) : base(data, iv) {

            RedisKey = redisKey;
            
        }

    }
}
