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
    public class ItemController : ControllerBase {

        /*
         * 
         * 특정 종류의 아이템에 대한 전체 정보 요청 (uid, type) 으로 gameDB에서 정보 확인, 해당 key로 masterDB에서 정보 조회
         * 
         */

        private readonly IItemService _itemService;
        private readonly IDataProcessService _dataProcessService;
        private readonly ILogger<ItemController> _logger;

        public ItemController(ILogger<ItemController> logger, IItemService itemService, IDataProcessService dataProcessService) {
            
            _logger = logger;
            _itemService = itemService;
            _dataProcessService = dataProcessService;
        
        }

        [HttpPost("list")]
        public async Task GetUserItemList() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoItemReq>(HttpContext);

            try {

                ProtocolRes res = await _itemService.GetItemList(req);

                if (res.Result == ErrorCode.None) {

                    _logger.ZLogInformation($"[Request get user item info list] UserID : {req.UserID} , Result:{res.Result}");
                
                }

                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            } catch (Exception ex) {

                _logger.ZLogError($"[Error in Request get user item info list]: {ex.ToString()}");
                BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

            }

        }


        [HttpPost("gatcha")]
        public async Task GetGatchaList() {

            (var req, var key) = _dataProcessService.GetDecryptedAndDeserializedData<DtoGatchaewardReq>(HttpContext);
            

                try {
                    
                    ProtocolRes res = await _itemService.GetGatchaReward(req);

                    if (res.Result == ErrorCode.None) {

                        _logger.ZLogInformation($"[Request get gatcha list] UserID : {req.UserID} , Result:{res.Result}");

                    }

                    await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

                } catch (Exception ex) {

                    _logger.ZLogError($"[Error in Request get gatcha list]: {ex.ToString()}");
                    BasicProtocolRes res = new BasicProtocolRes() { Result = ErrorCode.ServerUnhandleException };
                    await _dataProcessService.SendEecryptedAndSerializedData(res, Response, key);

                }

        }

    }

}
