using HttpServer.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ZLogger;

namespace HttpServer.Controllers {

    [Route("noneAuth/[controller]")]
    [ApiController]
    public class HandShakeController : ControllerBase {

        private readonly IHandShakeService _handShakeService;
        private readonly ILogger<HandShakeController> _logger;

        public HandShakeController(IHandShakeService handShakeService, ILogger<HandShakeController> logger) { 
            _handShakeService = handShakeService;
            _logger = logger;
        }

        [HttpPost]
        public async Task HandShakePost() {

            try {

                await _handShakeService.SendCommonPublicKey(HttpContext);

                _logger.ZLogInformation($"[Request HandShake]");

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request HandShake]: {ex.ToString()}");

            }

        }

    }

}
