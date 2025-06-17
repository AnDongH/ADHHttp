using HttpServer.DAO;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ADHNetworkShared.Protocol;
using ZLogger;
using System.Linq;
using ADHNetworkShared.Protocol.DTO;
using System.Transactions;

namespace HttpServer.Repository {
    public partial class GameDB : IGameDB {

        public async Task<(ErrorCode, DaoGdbAttendanceInfo)> GetAttendance(long uid) {

            var isExists = await _queryFactory
                           .Query("user_attendance")
                           .Where("uid", uid)
                           .ExistsAsync();

            if (!isExists) {

                using var transaction = _dbConn.BeginTransaction();

                // 만들기
                int affected = await _queryFactory
                                .Query("user_attendance")
                                .InsertAsync(new {
                                    uid = uid,
                                    attendance_cnt = 0,
                                    recent_attendance_dt = DateTime.MinValue
                                }, transaction);

                // 삽입 실패 시 예외 처리
                if (affected == 0) {
                    transaction.Rollback();
                    return (ErrorCode.SQLAffectedZero, null);

                }

                // 보상 리스트 가져오기
                List<int> dayList = (await GetMasterDB<DaoMasterDBAttendanceReward>("master_attendance_reward"))
                                                                .Select(att => att.day_seq)
                                                                .Distinct()
                                                                .ToList();

                // 유저 보상 테이블에 데이터 생성
                foreach (var day in dayList) {

                    affected = await _queryFactory
                                     .Query("user_attendance_reward")
                                     .InsertAsync(new { 
                                         uid = uid, 
                                         day_seq = day, 
                                         is_received = false 
                                     }, transaction);


                    if (affected == 0) {
                        transaction.Rollback();
                        return (ErrorCode.SQLAffectedZero, null);
                    }

                }

            }

            var result = await _queryFactory.Query("user_attendance").Where("uid", uid)
                                    .FirstOrDefaultAsync<DaoGdbAttendanceInfo>();

            return result != null ? (ErrorCode.None, result) : (ErrorCode.SQLInfoDoesNotExist, result);

        }

        // 트랜잭션에서 데드락이 발생할 수 있다. 주의하자
        public async Task<ErrorCode> SetAttendance(long uid) {

            // 출석 정보 존재함?
            var isExists = await _queryFactory
                                       .Query("user_attendance")
                                       .Where("uid", uid)
                                       .ExistsAsync();

            int affected = 0;

            if (!isExists) {

                using var transaction = _dbConn.BeginTransaction();
                    // 만들기
                affected = await _queryFactory
                                .Query("user_attendance")
                                .InsertAsync(new {
                                    uid = uid,
                                    attendance_cnt = 0,
                                    recent_attendance_dt = DateTime.MinValue
                                }, transaction);

                // 삽입 실패 시 예외 처리
                if (affected == 0) {
                    transaction.Rollback();
                    return ErrorCode.SQLAffectedZero;

                }

                // 보상 리스트 가져오기
                List<int> dayList = (await GetMasterDB<DaoMasterDBAttendanceReward>("master_attendance_reward"))
                                                                .Select(att => att.day_seq)
                                                                .Distinct()
                                                                .ToList();

                // 유저 보상 테이블에 데이터 생성
                foreach (var day in dayList) {

                    affected = await _queryFactory
                                     .Query("user_attendance_reward")
                                     .InsertAsync(new {
                                         uid = uid,
                                         day_seq = day,
                                         is_received = false
                                     }, transaction);


                    if (affected == 0) {
                        transaction.Rollback();
                        return ErrorCode.SQLAffectedZero;
                    }

                }

                transaction.Commit();

            }


            var attendanceData = await _queryFactory
                    .Query("user_attendance")
                    .Where("uid", uid)
                    .FirstOrDefaultAsync<DaoGdbAttendanceInfo>();

            // 오늘 이미 출석 함?
            if (attendanceData.recent_attendance_dt.Date >= DateTime.UtcNow.Date) {
                //return ErrorCode.AlreadyAttendance;
            }

            // recent_attendance_dt를 현재 시각으로 업데이트
            affected = await _queryFactory
                            .Query("user_attendance")
                            .Where("uid", uid)
                            .UpdateAsync(new {
                                attendance_cnt = attendanceData.attendance_cnt + 1,
                                recent_attendance_dt = DateTime.UtcNow
                            });


            if (affected == 0) {
                return ErrorCode.SQLAffectedZero;
            }

            return ErrorCode.None;

        }


        public async Task<(ErrorCode, List<int>)> CheckAttendanceReward(DaoGdbAttendanceInfo info, long uid) {

            var result = await _queryFactory
                                    .Query("user_attendance_reward")
                                    .Where("uid", uid)
                                    .Where("day_seq", "<=", info.attendance_cnt)
                                    .Where("is_received", false)
                                    .Select("day_seq")
                                    .GetAsync<int>();

            return result != null ? (ErrorCode.None, result.ToList()) : (ErrorCode.SQLInfoDoesNotExist, result.ToList());

        }


        public async Task<ErrorCode> UpdateAttendanceRewardInfo(int day, long uid) {

            DaoGdbAttendanceReward userRewardData =  await _queryFactory
                                           .Query("user_attendance_reward")
                                           .Where("uid", uid)
                                           .Where("day_seq", day)
                                           .Select("is_received")
                                           .FirstOrDefaultAsync<DaoGdbAttendanceReward>();

            if (userRewardData.is_received) { return ErrorCode.AlreadyReceivedReward; }

            using (var transaction = _dbConn.BeginTransaction()) {

                int affected = await _queryFactory
                    .Query("user_attendance_reward")
                    .Where("uid", uid)
                    .Where("day_seq", day)
                    .UpdateAsync(new { is_received = true }, transaction);


                if (affected == 0) {
                    transaction.Rollback();
                    return ErrorCode.SQLFailException;
                }


                var masterMailReward = (await GetMasterDB<DaoMasterDBAttendanceReward>("master_attendance_reward"))
                                        .Where(att => att.day_seq == day);

                var masteritemList = await GetMasterItemInfos(true);

                foreach (var data in masterMailReward) {

                    ItemType type = (ItemType)masteritemList.Find(x => x.item_id == data.item_id).item_type_id;
                    string table = Router.UserItemTableMap[type];
                    ErrorCode code = await UpdateUserItem(table, transaction, uid, data.item_id, data.item_cnt);

                    if (code != ErrorCode.None) {
                        transaction.Rollback();
                        return code;
                    }
                }

                transaction.Commit();
                return ErrorCode.None;

            }


        }

    }

}
