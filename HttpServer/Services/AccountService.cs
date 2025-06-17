using ADHNetworkShared.Protocol.DTO;
using ADHNetworkShared.Protocol;
using HttpServer.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HttpServer.Repository.Interface;

namespace HttpServer.Services {
    public class AccountService : IAccountService {

        private readonly IGameDB _gameDB;
        private readonly IMemoryDB_Test _memoryDB;

        public AccountService(IGameDB gameDB, IMemoryDB_Test memoryDB) { 
            _gameDB = gameDB;
            _memoryDB = memoryDB;
        }

        public async Task<BasicProtocolRes> CreateUserAccount(DtoAccountRegisterReq request) {

            ErrorCode errorCode = ErrorCode.None;

            if (_gameDB.ActivateConnection()) {

                errorCode = await _gameDB.CreateUserAccount(request.ID, request.PW);

            } else {

                errorCode = ErrorCode.DBConnectionFailException;

            }

            return new BasicProtocolRes() { Result = errorCode };
        }

        public async Task<BasicProtocolRes> SetUserInfo(DtoAccountInfoReq request) {

            ErrorCode errorCode = ErrorCode.None;

            if (_gameDB.ActivateConnection()) {

                errorCode = await _gameDB.SetUserInfo(request.UserID, request.nick_name);

            } else {

                errorCode = ErrorCode.DBConnectionFailException;

            }

            return new BasicProtocolRes() { Result = errorCode };
        }

        // 문제가 있을 수 있나? 계정은 삭제했는데 로그아웃에 실패했다면? 어떻게 해야할까
        public async Task<BasicProtocolRes> DeleteUserAccount(DtoAccountDeleteReq request) {

            ErrorCode errorCode = ErrorCode.None;

            if (_gameDB.ActivateConnection()) {

                errorCode = await _gameDB.PWCheck(request.UserID, request.PW);

                if (errorCode != ErrorCode.None) return new BasicProtocolRes() { Result = errorCode };

                errorCode = await _gameDB.DeleteUserAccount(request.UserID);

                if (errorCode != ErrorCode.None) return new BasicProtocolRes() { Result = errorCode };

                errorCode = await _memoryDB.DeleteUserAsync(request.UserID);

                if (errorCode != ErrorCode.None) return new BasicProtocolRes() { Result = errorCode };

                errorCode = await _memoryDB.DeleteClientKeyAsync(request.UserID);

            } else {

                errorCode = ErrorCode.DBConnectionFailException;

            }

            return new BasicProtocolRes() { Result = errorCode };
        }
    }
}
