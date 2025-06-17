using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.DAO;
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
    public class PingController : ControllerBase {

        private readonly ILogger<PingController> _logger;
        private readonly IDataProcessService _dataProcessService;
        private readonly IPingService _pingService;

        public PingController(ILogger<PingController> logger, IDataProcessService dataProcessService, IPingService pingService) {

            _logger = logger;
            _dataProcessService = dataProcessService;
            _pingService = pingService;

        }

        [HttpPost("ping")]
        public async Task Ping() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<PingReq>(HttpContext);
            
            try {

                ProtocolRes res = await _pingService.UpdateTokenLife(req);

                if (res.Result == ErrorCode.None) {
                    _logger.ZLogInformation($"[Request ping] UserID : {req.UserID} , Result:{res.Result}");
                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in ping]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.PingFailException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }

    }

}
