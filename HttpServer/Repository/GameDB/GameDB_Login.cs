using HttpServer.DAO;
using SqlKata.Execution;
using System.Threading.Tasks;
using System;
using ZLogger;
using ADHNetworkShared.Protocol;

namespace HttpServer.Repository {
    public partial class GameDB : IGameDB {

        public async Task<(ErrorCode, long)> AuthCheck(string id, string pw) {
            
            var userInfo = await _queryFactory.Query("user_account")
                                    .Where("id", id)
                                    .Select("uid", "hashed_pw", "salt_value")
                                    .FirstOrDefaultAsync<DaoGdbUserAccount>();

            if (userInfo.hashed_pw != Security.MakeHashingPassWord(userInfo.salt_value, pw)) return (ErrorCode.LoginFailPwNotMatch, 0);


            int affected = await _queryFactory.Query("user_account")
                                    .Where("id", id)
                                    .UpdateAsync(new { recent_login_dt = DateTime.UtcNow });

            if (affected == 0) return (ErrorCode.SQLAffectedZero, 0);

            return (ErrorCode.None, userInfo.uid);
        
        }

        public async Task<ErrorCode> PWCheck(long uid, string pw) {

            var userInfo = await _queryFactory.Query("user_account")
                                              .Where("uid", uid)
                                              .Select("uid", "hashed_pw", "salt_value")
                                              .FirstOrDefaultAsync<DaoGdbUserAccount>();

            if (userInfo.hashed_pw != Security.MakeHashingPassWord(userInfo.salt_value, pw)) return ErrorCode.LoginFailPwNotMatch;

            return ErrorCode.None;
        
        }

    }

}
