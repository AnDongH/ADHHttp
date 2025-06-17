using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol
{
    // 1000 ~ 19999
    /*
     * 
     * 에러 코드 산정 규칙
     * 
     * 1. 예외 처리에서 처리하는 코드는 Exception 코드를 붙인다.
     * 
     * 2. 기능 단위로 코드를 처리한다. (redis나 sql은 서버 개발자한테 중요한 것이지, 클라이언트에게는 중요하지 않으므로)
     * 클라이언트에게 올바른 정보를 줄 수 있는 단위로 작성한다.
     * 
     * 3. 가능한 구체적인 코드를 남긴다. (클라이언트가 디버깅이 가능하도록)
     * 
     * 나중에 위 규칙을 따르도록 재정의 하자
     * 
     */



    public enum ErrorCode : UInt16 {
        None = 0,

        #region server error (exception 1 ~ 1000)
        
        ServerUnhandleException = 1,
        RedisFailException = 2,
        SQLFailException = 3,
        PingFailException = 4,
        DBConnectionFailException = 5,

        #endregion

        #region client error (invalid request 1001 ~ 19999)
        
        RedisHasNotKey = 1001,
        RedisHasNotValue = 1002,
        SQLAffectedZero = 1003,
        InValidRequestHttpBody = 1004,
        InValidAuthToken = 1005,
        AuthTokenNotExist = 1006,
        UserIDNotExist = 1007,
        AuthTokenKeyNotFound = 1008,
        LoginFailUserNotExist = 1009,
        LoginFailPwNotMatch = 1010,
        SQLInfoDoesNotExist = 1011,
        InValidVersion = 1012,
        LoginFailException = 1013,
        AlreadyReceivedReward = 1014,
        AlreadyRequestedOrRegisteredFriend = 1015,
        AlreadyAttendance = 1016,

        #endregion

    }
}
