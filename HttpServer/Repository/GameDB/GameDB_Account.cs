using SqlKata.Execution;
using System.Text;
using System.Threading.Tasks;
using System;
using ZLogger;
using ADHNetworkShared.Protocol;
using HttpServer.DAO;

namespace HttpServer.Repository {
    public partial class GameDB : IGameDB {

        public async Task<ErrorCode> CreateUserAccount(string userID, string userPw) {

            // userID가 존재하지 않으면 새로운 사용자를 삽입하고 uid를 바로 가져옴
            StringBuilder sb = new StringBuilder();
            sb.Append(Security.SaltString());
            long suid = Security.CreateSUID();


            int affected = await _queryFactory
                .Query("user_account")
                .InsertAsync(new {
                    uid = suid,
                    id = userID,
                    hashed_pw = Security.MakeHashingPassWord(sb.ToString(), userPw),
                    salt_value = sb.ToString(),
                    create_dt = DateTime.UtcNow,
                    recent_login_dt = default(DateTime)
                });

            sb.Clear();

            // 삽입 실패 시 예외 처리
            if (affected == 0) return ErrorCode.SQLAffectedZero;

            affected = await _queryFactory
                             .Query("user_info")
                             .InsertAsync(new {
                                 uid = suid,
                                 nick_name = $"USER-{suid}",
                                 create_dt = DateTime.UtcNow
                             });

            // 삽입 실패 시 예외 처리
            if (affected == 0) return ErrorCode.SQLAffectedZero;


            await _logDB.CreateLogAccount(suid);

            return ErrorCode.None;

        }

        public async Task<(ErrorCode, DaoGdbUserInfo)> GetUserInfo(long uid) {

            var data = await _queryFactory
                             .Query("user_info")
                             .Where("uid", uid)
                             .Select("uid", "nick_name")
                             .FirstOrDefaultAsync<DaoGdbUserInfo>();

            return data == null ? (ErrorCode.SQLInfoDoesNotExist, null) : (ErrorCode.None, data);

        }

        public async Task<ErrorCode> SetUserInfo(long uid, string nick_name) {

            int affected = await _queryFactory
                                 .Query("user_info")
                                 .Where("uid", uid)
                                 .UpdateAsync(new { nick_name = nick_name });

            return affected == 0 ? ErrorCode.SQLAffectedZero : ErrorCode.None;

        }

        // select 결과가 -> update에 큰 영향을 미친다면 트랜잭션을 사용해야 한다.
        public async Task<ErrorCode> DeleteUserAccount(long uid) {

            int affected = await _queryFactory.Query("user_account")
                                              .Where("uid", uid)
                                              .DeleteAsync();

            if (affected == 0) return ErrorCode.SQLAffectedZero;

            return ErrorCode.None;

        }

    }

}
