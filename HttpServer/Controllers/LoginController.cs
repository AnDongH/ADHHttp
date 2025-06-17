using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using ZLogger;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.Services;
using HttpServer.Repository;
using HttpServer.DAO;
using ADHNetworkShared.Protocol;

namespace HttpServer.Controllers {

    // ApiController 특성은 해당 클래스가 API 컨트롤러라는 것을 나타낸다.
    // 모델 바인딩, 유효성 검사 등의 자동화된 처리를 제공한다.

    // Route 특성은 실제로 해당 요청이 들어오게 되는 경로를 말한다. 기본적으로 string 형태이며
    // [controller] 토큰을 사용하면 저절로 LoginController라는 이름에서 Controller를 빼고 Login -> login으로 변경해서 /login 경로를 사용한다.
    // 따라서 앵간하면 무조건 LoginController 이런식으로 접미사를 붙이자. 그리고 얘들은 ControllerBase를 상속받는다.
    // HttpPost 특성은 Post요청이 해당 컨트롤러에 도달하면 해당 메서드를 실행시키겠다는 특성이다.

    [Route("noneAuth/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase {

        private readonly ILogger<LoginController> _logger;
        private readonly ILoginService _loginService;
        private readonly IAttendanceService _attendanceService;
        private readonly IDataProcessService _dataProcessService;

        private readonly ILogDB _logDB;

        public LoginController(ILogger<LoginController> logger, ILoginService loginService, IAttendanceService attendanceService, IDataProcessService dataProcessService, ILogDB logDB) {
            _logger = logger;
            _loginService = loginService;
            _attendanceService = attendanceService;
            _dataProcessService = dataProcessService;
            _logDB = logDB;
        }

        
        [HttpPost]
        public async Task Post() {
            
            if (!ModelState.IsValid) {
                // 여기에서 "데이타 유효성 검증" 을 통과하지 못한 것에 대한 처리를 해줘야 한다.
                // 예를 들어서 에러를 내보내게 하는 곳으로 리디렉션을 한다던가..
                // 아니면 그냥 에러코드랑 함께 응답을 리턴해준다던가
                // 근데 [ApiController] 어트리뷰트를 붙이면 알아서 400 Bad Request를 보내준다. 
            }

            var (req, key) = _dataProcessService.GetDecryptedAndDeserializedNoneAuthData<DtoLoginReq>(HttpContext);

            try {

                ProtocolRes res = await _loginService.GetLoginResponse(req);

                if (res.Result == ErrorCode.None) {

                    _logger.ZLogInformation($"[Request Login] ID:{req.ID}, PW:{req.PW}");
                    //await _logDB.LogToDB(new DaoLog(((DtoLoginRes)res).Uid, "information", DateTime.UtcNow), "login_log");
                    
                }

                await _dataProcessService.SendEecryptedAndSerializedNoneAuthData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request Login]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedNoneAuthData(res, Response, key);

            }
        
        }
    }
}
