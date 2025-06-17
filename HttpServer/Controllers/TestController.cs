using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using ZLogger;
using System.IO;
using HttpServer.Services;
using ADHNetworkShared.Protocol.DTO;
using ADHNetworkShared.Crypto;
using Microsoft.AspNetCore.Http.Timeouts;
using HttpServer.DAO;
using HttpServer.Repository;
using ADHNetworkShared.Protocol;
using System.Diagnostics;

namespace HttpServer.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        private readonly ILogger<TestController> _logger;
        private readonly IDataProcessService _dataProcessService;
        private readonly ITestService _testService;

        //private readonly ILogDB _logDB;

        public TestController(ILogger<TestController> logger, IDataProcessService dataProcessService, ITestService testService /*ILogDB logDB*/) {
            _logger = logger;
            _dataProcessService = dataProcessService;
            _testService = testService;
            //_logDB = logDB;
        }

        [HttpGet("/noneAuth/byteTest")]
        public IActionResult GetBytes() {

            string s = Summaries[Random.Shared.Next(0, Summaries.Length)];
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);

            _logger.ZLogInformation($"byte length: {bytes.Length}");

            return File(bytes, "application/octet-stream");
        }

        [HttpGet("/noneAuth/responseDirectTest")]
        public async void GetResponseDirect() {

            string s = Summaries[Random.Shared.Next(0, Summaries.Length)];
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);

            _logger.ZLogInformation($"byte length: {bytes.Length}");

            await using (Stream st = Response.Body) {
                await st.WriteAsync(bytes);                
            }

        }

        // Ÿ�� �ƿ��� �����ϰ� ���� ���� Inner Method�� ���ΰ�, await InnsePostTest(HttpContext).WithCancellation(HttpContext.RequestAborted);
        // [RequestTimeout(milliseconds: 5000)] ��Ʈ����Ʈ ���
        private async Task InnsePostTest(HttpContext httpContext) {

            var (req, key) = _dataProcessService.GetDecryptedAndDeserializedNoneAuthData<PostTestReq>(httpContext);

            ProtocolRes res = _testService.PostTest(req);

            // timeout test�� delay
           // await Task.Delay(100000);

            await _dataProcessService.SendEecryptedAndSerializedNoneAuthData(res, Response, key);

            _logger.ZLogInformation($"[Request PostTest]: Message - {req.requestMSG}");

        }

        [HttpPost("/noneAuth/PostTest")]
        //[RequestTimeout(milliseconds: 5000)]
        public async Task PostTest() {

            try {

                await InnsePostTest(HttpContext).WithCancellation(HttpContext.RequestAborted);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request PostTest]: {ex.ToString()}");

            }
        
        }


        [HttpPost("AuthPostTest")]
        public async Task AuthPostTest() {

            try {

                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();

                var (req, key) = _dataProcessService.GetDecryptedAndDeserializedData<AuthPostTestReq>(HttpContext);


                ProtocolRes res = _testService.AuthPostTest(req);
                
                _logger.ZLogInformation($"[Request AuthPostTest]: Message - {req.requestMSG}");

                //await _logDB.LogToDB(new DaoLog(req.UserID, "information", DateTime.UtcNow), "post_log");

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);
                    
                stopwatch.Stop();
                /////////////////////////////// ���� �׽�Ʈ�� ////////////////////////////////////////
                //Console.WriteLine($"[Request AuthPostTest]: Message - {req.requestMSG} - {stopwatch.Elapsed.TotalMilliseconds}ms");
                //////////////////////////////////////////////////////////////////////////////////////
                stopwatch.Reset();


            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request AuthPostTest]: {ex.ToString()}");

            }

        }


        [HttpPost("itemlist")]
        public async Task GetItemList() {
        
            try {
        
                var (req, key) = _dataProcessService.GetDecryptedAndDeserializedData<ItemDBTestReq>(HttpContext);

                ProtocolRes res = await _testService.ItemDBTest(req);
                
                _logger.ZLogInformation($"[Request GetItemList]: UID - {req.UserID}, Result - {res.Result}");
        
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key, true);
        
            } catch (Exception ex) {
        
                _logger.ZLogError($"[Error in Request GetItemList]: {ex.ToString()}");
        
            }
        
        }

        [HttpPost("updateitemlist")]
        public async Task UpdateItemList() {

            try {

                var (req, key) = _dataProcessService.GetDecryptedAndDeserializedData<ItemDBUpdateTestReq>(HttpContext);

                ProtocolRes res = await _testService.ItemDBUpdateTest(req);
                
                _logger.ZLogInformation($"[Request UpdateItemList]: UID - {req.UserID}, Result - {res.Result}");

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key, true);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request GetItemList]: {ex.ToString()}");

            }

        }


    }

}
