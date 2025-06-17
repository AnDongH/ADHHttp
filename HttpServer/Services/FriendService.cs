using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.DAO;
using HttpServer.Repository;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HttpServer.Services {
    public class FriendService : IFriendService {

        private readonly IGameDB _gameDB;

        public FriendService(IGameDB gameDB) {
            _gameDB = gameDB;
        }

        public async Task<DtoFriendInfoListRes> GetFriendInfoList(DtoFriendInfoListReq request) {

            var res = new DtoFriendInfoListRes();

            if (_gameDB.ActivateConnection()) {

                (res.Result, var list) = await _gameDB.GetFriendInfoList(request.UserID);

                if (res.Result != ErrorCode.None) return res;

                res.userDatas = new List<UserData>();

                foreach (var item in list) {

                    long target = item.uid == request.UserID ? item.friend_uid : item.uid;

                    (res.Result, DaoGdbUserInfo info) = await _gameDB.GetUserInfo(target);

                    if (res.Result != ErrorCode.None) return res;

                    res.userDatas.Add(new UserData(info.uid, info.nick_name));

                }

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;

        }

        public async Task<DtoFriendInfoListRes> GetFriendReqInfoList(DtoFriendReqInfoListReq request) {

            var res = new DtoFriendInfoListRes();
            
            if (_gameDB.ActivateConnection()) {

                (res.Result, var list) = await _gameDB.GetFriendReqInfo(request.UserID);

                if (res.Result != ErrorCode.None) return res;

                res.userDatas = new List<UserData>();

                foreach (var item in list) {

                    (res.Result, DaoGdbUserInfo info) = await _gameDB.GetUserInfo(item.friend_uid);

                    if (res.Result != ErrorCode.None) return res;

                    res.userDatas.Add(new UserData(info.uid, info.nick_name));

                }

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;

        }

        public async Task<DtoFriendInfoListRes> GetFriendReceivedInfoList(DtoFriendReceivedInfoListReq request) {

            var res = new DtoFriendInfoListRes();

            if (_gameDB.ActivateConnection()) {


                (res.Result, var list) = await _gameDB.GetFriendReceivedInfo(request.UserID);

                if (res.Result != ErrorCode.None) return res;

                res.userDatas = new List<UserData>();

                foreach (var item in list) {

                    (res.Result, DaoGdbUserInfo info) = await _gameDB.GetUserInfo(item.uid);

                    if (res.Result != ErrorCode.None) return res;

                    res.userDatas.Add(new UserData(info.uid, info.nick_name));

                }

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;

        }

        public async Task<BasicProtocolRes> InsertFriendReq(DtoFriendReqReq request) {

            var res = new BasicProtocolRes();

            if (_gameDB.ActivateConnection()) {

                res.Result = await _gameDB.InsertFriendReq(request.UserID, request.friend_uid);

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;

        }

        public async Task<BasicProtocolRes> AcceptFriendReq(DtoFriendAcceptReq request) {

            var res = new BasicProtocolRes();

            if (_gameDB.ActivateConnection()) {

                res.Result = await _gameDB.AcceptFriendReq(request.UserID, request.friend_uid);

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;

        }

        public async Task<BasicProtocolRes> DeleteFriend(DtoFriendDeleteReq request) {

            var res = new BasicProtocolRes();

            if (_gameDB.ActivateConnection()) {

                res.Result = await _gameDB.DeleteFriend(request.UserID, request.friend_uid);

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;
        
        }

        public async Task<BasicProtocolRes> CancelFriendReq(DtoFriendReqCancelReq request) {

            var res = new BasicProtocolRes();

            if (_gameDB.ActivateConnection()) {

                res.Result = await _gameDB.CancelOrDenyFriendReq(request.UserID, request.friend_uid);

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;

        }

        public async Task<BasicProtocolRes> DenyFriendReq(DtoFriendReqDenyReq request) {

            var res = new BasicProtocolRes();

            if (_gameDB.ActivateConnection()) {

                res.Result = await _gameDB.CancelOrDenyFriendReq(request.friend_uid, request.UserID);

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            } 

            return res;

        }
    }

}
