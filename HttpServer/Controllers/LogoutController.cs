using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.DAO;
using HttpServer.Repository;
using HttpServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Threading.Tasks;
using ZLogger;

namespace HttpServer.Controllers {

    [Route("[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase {

        private readonly ILoginService _loginService;
        private readonly IDataProcessService _dataProcessService;
        private readonly ILogger<LogoutController> _logger;

        private readonly ILogDB _logDB;
        public LogoutController(ILoginService loginService, IDataProcessService dataProcessService, ILogger<LogoutController> logger, ILogDB logDB) {

            _loginService = loginService;
            _dataProcessService = dataProcessService;
            _logger = logger;
            _logDB = logDB;
        }

        [HttpPost]
        public async Task Post() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoLogoutReq>(HttpContext);
            
            try {

                ProtocolRes res = await _loginService.GetLogoutResponse(req);

                if (res.Result == ErrorCode.None) {

                    _logger.ZLogInformation($"[Request Logout]: UserID - {req.UserID}");
                    await _logDB.LogToDB(new DaoLog(req.UserID, "information", DateTime.UtcNow), "logout_log");
                
                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request Logout]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

    }

}
