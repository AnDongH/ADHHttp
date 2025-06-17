using ADHNetworkShared.Protocol;
using HttpServer.DAO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HttpServer.Repository.Interface {
    public interface IMemoryDB_Test : IDisposable {
        public Task<ErrorCode> RegistUserAsync(string authToken, long uid);
        public Task<ErrorCode> DeleteUserAsync(long uid);
        public Task<ErrorCode> UpdateUserAsync(long uid);
        public Task<(ErrorCode, DaoMdbUserData)> GetUserAsync(long uid);

        /// <summary>
        /// only temp key
        /// </summary>
        /// <param name="tempKey"></param>
        /// <param name="clientKey"></param>
        /// <returns></returns>
        public Task<ErrorCode> RegistClientKeyAsync(long kuid, byte[] clientKey);

        /// <summary>
        /// only uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Task<ErrorCode> DeleteClientKeyAsync(long kuid);

        /// <summary>
        /// create account and login is temp key also uid
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<(ErrorCode, byte[])> GetClientKeyAsync(long key);

        Task<(ErrorCode, List<T>)> GetCashedMasterData<T>(string key);
        Task<ErrorCode> CacheMasterData<T>(string key, List<T> values);
    }

}
