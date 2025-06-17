using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ADHNetworkShared.Protocol.DTO;
using System.Threading.Tasks;
using ZLogger;
using HttpServer.Services;
using System.IO;
using MemoryPack;
using ADHNetworkShared.Crypto;
using ADHNetworkShared.Protocol;
using Org.BouncyCastle.Asn1.Ocsp;
using System;

namespace HttpServer.Controllers {


    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase {

        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;
        private readonly IDataProcessService _dataProcessService;

        public AccountController(ILogger<AccountController> logger, IAccountService accountService, IDataProcessService dataProcessService) {

            _accountService = accountService;
            _logger = logger;
            _dataProcessService = dataProcessService;
        
        }

        [HttpPost("/noneAuth/[controller]/create")]
        public async Task CreatePost() {

            var (req, key) = _dataProcessService.GetDecryptedAndDeserializedNoneAuthData<DtoAccountRegisterReq>(HttpContext);

            try {

                ProtocolRes res = await _accountService.CreateUserAccount(req);

                _logger.ZLogInformation($"[Request Register] ID:{req.ID}, PW:{req.PW}");
                await _dataProcessService.SendEecryptedAndSerializedNoneAuthData(res, Response, key);

            } catch (Exception ex) {
                
                _logger.ZLogError($"[Error in Request Register]: {ex.ToString()}");

                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedNoneAuthData(res, Response, key);

            }
        }

        [HttpPost("info")]
        public async Task SetInfo() {

            var (req, key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoAccountInfoReq>(HttpContext);

            try {

                ProtocolRes res = await _accountService.SetUserInfo(req);

                _logger.ZLogInformation($"[Request set info] ID:{req.UserID}, PW:{res.Result}");

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request set info]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

        [HttpPost("delete")]
        public async Task DeletePost() {

            var (req, key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoAccountDeleteReq>(HttpContext);

            try {

                ProtocolRes res = await _accountService.DeleteUserAccount(req);
                
                _logger.ZLogInformation($"[Request Delete] ID:{req.UserID}, PW:{req.PW}");
                
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request Delete]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

    }

}
