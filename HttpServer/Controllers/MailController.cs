using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.DAO;
using HttpServer.Repository;
using HttpServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ZLogger;

namespace HttpServer.Controllers {
    
    [Route("[controller]")]
    [ApiController]
    public class MailController : ControllerBase {

        private readonly IMailService _mailService;
        private readonly IDataProcessService _dataProcessService;
        private readonly ILogger<MailController> _logger;
        private readonly ILogDB _logDB;

        public MailController(IMailService mailService, IDataProcessService dataProcessService, ILogger<MailController> logger, ILogDB logDB) { 
        
            _mailService = mailService;
            _dataProcessService = dataProcessService;
            _logger = logger;
            _logDB = logDB;

        }


        [HttpPost("get")]
        public async Task GetMailList() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoMailListReq>(HttpContext);

            try {

                ProtocolRes res = await _mailService.GetMailList(req);

                if (res.Result == ErrorCode.None) {

                    _logger.ZLogInformation($"[Request Get Mail List] UserID : {req.UserID} , Result:{res.Result}");
             
                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request Get Mail List]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

        [HttpPost("reward")]
        public async Task GetMailReward() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoMailRewardReq>(HttpContext);
            
            try {

                ProtocolRes res = await _mailService.ReceiveMailReward(req);

                if (res.Result == ErrorCode.None) {
                    
                    _logger.ZLogInformation($"[Request Mail RewardInfo] UserID : {req.UserID} , Result:{res.Result}");
                    await _logDB.LogToDB(new DaoRewardLog(req.UserID, req.mail_id, "Information", DateTime.UtcNow), "mail_reward_log");
                
                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request Mail RewardInfo]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

        [HttpPost("delete")]
        public async Task DeleteMail() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoMailDeleteReq>(HttpContext);
            
            try {

                ProtocolRes res = await _mailService.DeleteMail(req);

                if (res.Result == ErrorCode.None) {

                    _logger.ZLogInformation($"[Request Delete Mail] UserID : {req.UserID} , Result:{res.Result}");
                
                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request Delete Mail]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

    }

}
