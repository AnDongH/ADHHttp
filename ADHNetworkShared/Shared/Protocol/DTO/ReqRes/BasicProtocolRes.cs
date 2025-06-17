using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {


    // 기본적으로 응답 메시지로 결과 코드 빼고 아무것도 넣을 필요가 없다면 해당 DTO를 사용한다.
    [MemoryPackable]
    public partial class BasicProtocolRes : ProtocolRes {
            


    }


}
