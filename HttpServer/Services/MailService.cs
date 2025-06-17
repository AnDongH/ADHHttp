using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HttpServer.Services {
    public class MailService : IMailService {

        private readonly IGameDB _gameDB;

        public MailService(IGameDB gameDB) { 
        
            _gameDB = gameDB;
        
        }

        public async Task<BasicProtocolRes> DeleteMail(DtoMailDeleteReq request) {
            
            var res = new BasicProtocolRes();

            if (_gameDB.ActivateConnection()) {

                res.Result = await _gameDB.DeleteMail(request.UserID, request.mail_id);
                
            } else {

                res.Result = ErrorCode.DBConnectionFailException;
            
            }

            return res;

        }

        public async Task<DtoMailListRes> GetMailList(DtoMailListReq request) {

            var res = new DtoMailListRes();

            if (_gameDB.ActivateConnection()) {

                (res.Result, res.mailInfos) = await _gameDB.GetMailList(request.UserID);

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;

        }

        public async Task<DtoRewardRes> ReceiveMailReward(DtoMailRewardReq request) {

            var res = new DtoRewardRes();

            if (_gameDB.ActivateConnection()) {

                res.Result = await _gameDB.CheckMailRewardReceived(request.UserID, request.mail_id);

                if (res.Result != ErrorCode.None) return res;

                (res.Result, res.rewardInfos) = await _gameDB.UpdateMailRewardInfo(request.mail_id, request.UserID);

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;
        }
    
    }

}
