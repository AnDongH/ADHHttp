using HttpServer.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.Services;
using System;
using HttpServer.DAO;
using ADHNetworkShared.Protocol;

namespace HttpServer.Controllers {

    [Route("[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase {

        private readonly ILogger<AttendanceController> _logger;
        private readonly IAttendanceService _attendanceService;
        private readonly IDataProcessService _dataProcessService;
        private readonly ILogDB _logDB;

        public AttendanceController(ILogger<AttendanceController> logger, IAttendanceService attendanceService, IDataProcessService dataProcessService, ILogDB logDB) {
           
            _logger = logger;
            _attendanceService = attendanceService;
            _dataProcessService = dataProcessService;
            _logDB = logDB; 

        }

        /// <summary>
        /// 출석 정보 API </br>
        /// 유저의 출석 정보(누적 출석일, 최근 출석 일시)를 전달합니다.
        /// </summary>
        [HttpPost("getinfo")]
        //[Authorize("Auth")] // [Authorize] 만 쓰면 기본 인증 절차만 확인하겠단 소리!!
        public async Task GetInfo() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoAttendanceGetReq>(HttpContext);

            try {
                
                ProtocolRes res = await _attendanceService.GetAttendance(req);

                _logger.ZLogInformation($"[Request AttendanceInfo] UserID : {req.UserID} , Result:{res.Result}");

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request AttendanceInfo]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

        [HttpPost("setinfo")]
        public async Task SetInfo() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoAttendanceSetReq>(HttpContext);
            
            try {

                ProtocolRes res = await _attendanceService.SetAttendance(req);

                _logger.ZLogInformation($"[Request Set Attendance] UserID : {req.UserID} , Result:{res.Result}");
                await _logDB.LogToDB(new DaoLog(req.UserID, "information", DateTime.UtcNow), "set_attendance_log");

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request Set Attendance]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

        [HttpPost("check")]
        public async Task Check() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoAttendanceCheckReq>(HttpContext);
            
            try {

                ProtocolRes res = await _attendanceService.CheckAttendanceReward(req);

                _logger.ZLogInformation($"[Request Check Attendance] UserID : {req.UserID} , Result:{res.Result}");

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request Check Attendance]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

        [HttpPost("reward")]
        public async Task Reward() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoAttendanceRewardReq>(HttpContext);
            
            try {

                ProtocolRes res = await _attendanceService.GetReward(req);

                if (res.Result == ErrorCode.None) {
                    _logger.ZLogInformation($"[Request Attendance RewardInfo] UserID : {req.UserID} , Result:{res.Result}");
                    await _logDB.LogToDB(new DaoRewardLog(req.UserID, req.day, "Information", DateTime.UtcNow), "attendance_reward_log");
                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request Attendance RewardInfo]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

    }

}
