using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.DAO;
using HttpServer.Repository;
using HttpServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ZLogger;

namespace HttpServer.Controllers {

    [Route("[controller]")]
    [ApiController]
    public class FriendController : ControllerBase {
        
        private readonly IDataProcessService _dataProcessService;
        private readonly IFriendService _friendService;
        private readonly ILogger<FriendController> _logger;
        private readonly ILogDB _logDB;

        public FriendController(IDataProcessService dataProcessService ,IFriendService friendService, ILogger<FriendController> logger, ILogDB logDB) {

            _dataProcessService = dataProcessService;
            _friendService = friendService;
            _logger = logger;
            _logDB = logDB;
        
        }

        [HttpPost("info")]
        public async Task GetFriendInfoList() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoFriendInfoListReq>(HttpContext);
            
            try {

                ProtocolRes res = await _friendService.GetFriendInfoList(req);

                if (res.Result == ErrorCode.None) {

                    _logger.ZLogInformation($"[Request get friend info list] UserID : {req.UserID} , Result:{res.Result}");

                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request  get friend info list]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

        [HttpPost("reqinfo")]
        public async Task GetFriendReqInfoList() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoFriendReqInfoListReq>(HttpContext);
            
            try {

                ProtocolRes res = await _friendService.GetFriendReqInfoList(req);

                if (res.Result == ErrorCode.None) {

                    _logger.ZLogInformation($"[Request get friend request info list] UserID : {req.UserID} , Result:{res.Result}");

                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request  get friend request info list]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

        [HttpPost("rcvinfo")]
        public async Task GetFriendReceivedInfoList() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoFriendReceivedInfoListReq>(HttpContext);
            
            try {

                ProtocolRes res = await _friendService.GetFriendReceivedInfoList(req);

                if (res.Result == ErrorCode.None) {

                    _logger.ZLogInformation($"[Request get friend received info list] UserID : {req.UserID} , Result:{res.Result}");

                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request  get friend received info list]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

        [HttpPost("req")]
        public async Task InsertFriendReq() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoFriendReqReq>(HttpContext);
            
            try {

                ProtocolRes res = await _friendService.InsertFriendReq(req);

                if (res.Result == ErrorCode.None) {

                    _logger.ZLogInformation($"[Request friend] UserID : {req.UserID} , Result:{res.Result}");
                    await _logDB.LogToDB(new DaoFriendLog(req.UserID, req.friend_uid, DaoFriendLog.Req_Type.request, "Information", DateTime.UtcNow), "friend_log");

                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request friend]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

        [HttpPost("acc")]
        public async Task AcceptFriendReq() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoFriendAcceptReq>(HttpContext);
            
            try {

                ProtocolRes res = await _friendService.AcceptFriendReq(req);

                if (res.Result == ErrorCode.None) {

                    _logger.ZLogInformation($"[Request accept friend] UserID : {req.UserID} , Result:{res.Result}");
                    await _logDB.LogToDB(new DaoFriendLog(req.UserID, req.friend_uid, DaoFriendLog.Req_Type.accept, "Information", DateTime.UtcNow), "friend_log");

                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request accept friend]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

        [HttpPost("del")]
        public async Task DeleteFriend() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoFriendDeleteReq>(HttpContext);
            
            try {

                ProtocolRes res = await _friendService.DeleteFriend(req);

                if (res.Result == ErrorCode.None) {

                    _logger.ZLogInformation($"[Request delete friend] UserID : {req.UserID} , Result:{res.Result}");
                    await _logDB.LogToDB(new DaoFriendLog(req.UserID, req.friend_uid, DaoFriendLog.Req_Type.delete, "Information", DateTime.UtcNow), "friend_log");
                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request delete friend]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

        [HttpPost("cancel")]
        public async Task CancelFriendReq() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoFriendReqCancelReq>(HttpContext);
            
            try {

                ProtocolRes res = await _friendService.CancelFriendReq(req);

                if (res.Result == ErrorCode.None) {

                    _logger.ZLogInformation($"[Request cancel friend req] UserID : {req.UserID} , Result:{res.Result}");
                    await _logDB.LogToDB(new DaoFriendLog(req.UserID, req.friend_uid, DaoFriendLog.Req_Type.cancel, "Information", DateTime.UtcNow), "friend_log");
                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request cancel friend req]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

        [HttpPost("deny")]
        public async Task DenyFriendReq() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoFriendReqDenyReq>(HttpContext);
            
            try {

                ProtocolRes res = await _friendService.DenyFriendReq(req);

                if (res.Result == ErrorCode.None) {

                    _logger.ZLogInformation($"[Request deny friend req] UserID : {req.UserID} , Result:{res.Result}");
                    await _logDB.LogToDB(new DaoFriendLog(req.UserID, req.friend_uid, DaoFriendLog.Req_Type.deny, "Information", DateTime.UtcNow), "friend_log");

                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request deny friend req]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

    }

}
