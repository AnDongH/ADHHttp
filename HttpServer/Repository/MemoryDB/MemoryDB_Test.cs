
using ADHNetworkShared.Protocol;
using HttpServer.DAO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ADHRedis;
using ADHRedis.Converter;
using ZLogger;
using HttpServer.Repository.Interface;

namespace HttpServer.Repository {
    public class MemoryDB_Test : IMemoryDB_Test {

        private readonly ADHRedisConnection _redisConn;
        private readonly ILogger<MemoryDB_Test> _logger;

        public MemoryDB_Test(ILogger<MemoryDB_Test> logger, IOptions<DBOptions> dbConfig) {

            _logger = logger;

            _redisConn = new ADHRedisConnection(dbConfig.Value.Redis , 6379, new ADHMemoryPackConverter());

        }

        public void Dispose() {
            _redisConn.Disconnect();
        }


        public async Task<ErrorCode> RegistUserAsync(string authToken, long uid) {

            string key = MemoryDbKeyMaker.MakeUserDataKey(uid.ToString());
            ErrorCode result = ErrorCode.None;

            DaoMdbUserData user = new() {
                AuthToken = authToken,
                UId = uid,
            };

            

            var expiryTime = TimeSpan.FromMinutes((60 * 3));
            RedisSender<DaoMdbUserData> redis = new(_redisConn, key);

            if (await redis.SetAsync(user, expiryTime) == false) {
                result = ErrorCode.RedisHasNotKey;
                return result;
            }

            return result;
        }

        public async Task<(ErrorCode, DaoMdbUserData)> GetUserAsync(long uid) {

            var userIDKey = MemoryDbKeyMaker.MakeUserDataKey(uid.ToString());

            RedisSender<DaoMdbUserData> redis = new(_redisConn, userIDKey);
            DaoMdbUserData user = await redis.GetAsync();

            if (user == null) {

                _logger.ZLogError($"[GetUserAsync] UID = {userIDKey}, ErrorMessage = Not Assigned User, RedisString get Error");
                return (ErrorCode.LoginFailUserNotExist, null);

            }

            return (ErrorCode.None, user);

        }

        public async Task<ErrorCode> RegistClientKeyAsync(long kuid, byte[] clientKey) {

            string key = MemoryDbKeyMaker.MakeClientKeyKey(kuid.ToString());
            ErrorCode result = ErrorCode.None;

            var expiryTime = TimeSpan.FromMinutes((60 * 3));
            RedisSender<byte[]> redis = new(_redisConn, key);
            if (await redis.SetAsync(clientKey, expiryTime) == false) {
                result = ErrorCode.RedisHasNotKey;
            }

            return result;
        }

        public async Task<(ErrorCode, byte[])> GetClientKeyAsync(long redisKey) {
            var key = MemoryDbKeyMaker.MakeClientKeyKey(redisKey.ToString());

            RedisSender<byte[]> redis = new(_redisConn, key);
            byte[] clientKey = await redis.GetAsync();

            if (clientKey == null) {
                _logger.ZLogError(
                    $"[GetClientKeyAsync] KUID = {key}, ErrorMessage = Not Assigned Key, RedisString get Error");
                return (ErrorCode.RedisHasNotKey, null);
            }

            return (ErrorCode.None, clientKey);

        }

        public async Task<ErrorCode> DeleteUserAsync(long uid) {

            var userIDKey = MemoryDbKeyMaker.MakeUserDataKey(uid.ToString());

            RedisSender<DaoMdbUserData> redis = new(_redisConn, userIDKey);
            if (await redis.DelAsync() == false) {
                return ErrorCode.RedisHasNotKey;
            }

            return ErrorCode.None;

        }

        public async Task<ErrorCode> UpdateUserAsync(long uid) {

            var userIDKey = MemoryDbKeyMaker.MakeUserDataKey(uid.ToString());
            var key = MemoryDbKeyMaker.MakeClientKeyKey(uid.ToString());

            RedisSender<DaoMdbUserData> redisU = new(_redisConn, userIDKey);
            if (await redisU.ExpireAsync(TimeSpan.FromMinutes(60 * 3)) == false) {
                return ErrorCode.RedisHasNotKey;
            }

            RedisSender<byte[]> redisK = new(_redisConn, key);
            if (await redisK.ExpireAsync(TimeSpan.FromMinutes((60 * 3))) == false) {
                return ErrorCode.RedisHasNotKey;
            }

            return ErrorCode.None;


        }


        public async Task<ErrorCode> DeleteClientKeyAsync(long kuid) {

            var key = MemoryDbKeyMaker.MakeClientKeyKey(kuid.ToString());

            RedisSender<byte[]> redis = new(_redisConn, key);
            if (await redis.DelAsync() == false) {
                return ErrorCode.RedisHasNotKey;
            }

            return ErrorCode.None;

        }

        public async Task<ErrorCode> CacheMasterData<T>(string key, List<T> values) {

            var masterKey = MemoryDbKeyMaker.MakeMasterCacheKey(key);
            ErrorCode result = ErrorCode.None;

            var expiryTime = TimeSpan.FromSeconds(60 * 60 * 24 + Random.Shared.Next(0, 11)); // jitter
            RedisSender<List<T>> redis = new(_redisConn, masterKey);

            if (await redis.SetAsync(values, expiryTime) == false) {
                result = ErrorCode.RedisHasNotKey;
            }

            return result;

        }

        public async Task<(ErrorCode, List<T>)> GetCashedMasterData<T>(string key) {
            var masterKey = MemoryDbKeyMaker.MakeMasterCacheKey(key);

            RedisSender<List<T>> redis = new(_redisConn, masterKey);
            List<T> data = await redis.GetAsync();

            if (data == null) {

                _logger.ZLogError($"[Get cached master data] key = {masterKey}, ErrorMessage = Not Assigned User, RedisString get Error");
                return (ErrorCode.LoginFailUserNotExist, null);

            }

            return (ErrorCode.None, data);

        }

    }

}
