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
    public class ScoreController : ControllerBase {
        
        private readonly IRankingService _rankingService;
        private readonly ILogger<ScoreController> _logger;
        private readonly IDataProcessService _dataProcessService;
        private readonly ILogDB _logDB;

        public ScoreController(IRankingService rankingService, ILogger<ScoreController> logger, IDataProcessService dataProcessService, ILogDB logDB) {
            _rankingService = rankingService;
            _logger = logger;
            _dataProcessService = dataProcessService;
            _logDB = logDB;
        }

        [HttpPost("set")]
        public async Task SetScore() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoScoreSetReq>(HttpContext);

            try {

                ProtocolRes res = await _rankingService.SetScore(req);

                if (res.Result == ErrorCode.None) {

                    _logger.ZLogInformation($"[Request Set Score] UserID : {req.UserID} , Result:{res.Result}");
                    await _logDB.LogToDB(new DaoScoreLog(req.UserID, req.score, "information", DateTime.UtcNow), "set_score_log");

                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request Set Score]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

        [HttpPost("get/my")]
        public async Task GetMyRanking() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoGetMyRankingReq>(HttpContext);
            
            try {

                ProtocolRes res = await _rankingService.GetMyRanking(req);

                _logger.ZLogInformation($"[Request get my ranking] UserID : {req.UserID} , Result:{res.Result}");

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Request get my ranking]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

        [HttpPost("get/all")]
        public async Task GetAllRankings() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoGetAllRankingReq>(HttpContext);
            
            try {

                ProtocolRes res = await _rankingService.GetAllRankings(req);

                _logger.ZLogInformation($"[Request get all rankings] UserID : {req.UserID} , Result:{res.Result}");

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Request get all rankings]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

    }

}
