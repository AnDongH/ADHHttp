using ADHNetworkShared.Protocol.DTO;
using ADHNetworkShared.Protocol;
using HttpServer.Repository;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HttpServer.Services {
    public class AttendanceService : IAttendanceService {

        private readonly IGameDB _gameDB;

        public AttendanceService(IGameDB gameDB) { 
            _gameDB = gameDB;
        }

        public async Task<BasicProtocolRes> SetAttendance(DtoAttendanceSetReq request) {

            var response = new BasicProtocolRes();

            if (_gameDB.ActivateConnection()) {

                response.Result = await _gameDB.SetAttendance(request.UserID);

            } else {

                response.Result = ErrorCode.DBConnectionFailException;

            }

            return response;

        }

        
        public async Task<DtoAttendanceGetRes> GetAttendance(DtoAttendanceGetReq request) {
            
            var response = new DtoAttendanceGetRes();

            if (_gameDB.ActivateConnection()) {

                (response.Result, var info) = await _gameDB.GetAttendance(request.UserID);

                if (response.Result != ErrorCode.None) return response;

                response.uid = info.uid;
                response.attendance_cnt = info.attendance_cnt;
                response.recent_attendance_dt = info.recent_attendance_dt;

            } else {

                response.Result = ErrorCode.DBConnectionFailException;

            }

            return response;
        }


        // 출석의 모든 리워드들 정보와, 현재 받을 수 있는 날짜들 리턴
        public async Task<DtoAttendanceCheckRes> CheckAttendanceReward(DtoAttendanceCheckReq request) {

            var response = new DtoAttendanceCheckRes();

            if (_gameDB.ActivateConnection()) {

                // 출석 정보 가져오기
                (response.Result, var info) = await _gameDB.GetAttendance(request.UserID);

                if (response.Result != ErrorCode.None) return response;

                // 받을 수 있는 보상 날짜 가져오기
                (response.Result, response.receivableDays) = await _gameDB.CheckAttendanceReward(info, request.UserID);

                if (response.Result != ErrorCode.None) return response;

                // 모든 리워드 정보 가져오기
                (response.Result, response.allReward) = await _gameDB.GetAllAttendanceReward();

            } else {

                response.Result = ErrorCode.DBConnectionFailException;

            }

            return response;
        
        }
        
        // 특정 날짜 보상 가져오기
        public async Task<DtoRewardRes> GetReward(DtoAttendanceRewardReq request) {

            var res = new DtoRewardRes();

            if (_gameDB.ActivateConnection()) {

                res.Result = await _gameDB.UpdateAttendanceRewardInfo(request.day, request.UserID);

                if (res.Result != ErrorCode.None) return res;

                (res.Result, res.rewardInfos) = await _gameDB.GetAttendanceReward(request.day);

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;

        }
    
    }
}
