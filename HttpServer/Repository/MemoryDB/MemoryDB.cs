using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;
using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Logging;
using ZLogger;
using HttpServer.DAO;
using ADHNetworkShared.Protocol;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using StackExchange.Redis;

namespace HttpServer.Repository {
    public class MemoryDB : IMemoryDB {


        public RedisConnection _redisConn;
        private readonly ILogger<MemoryDB> _logger;

        public MemoryDB(ILogger<MemoryDB> logger, IOptions<DBOptions> dbConfig) {

            _logger = logger;

            RedisConfig config = new("default", dbConfig.Value.Redis);
            _redisConn = new RedisConnection(config, new MemoryPackConvertor());

        }

        public void Dispose() {
            _redisConn?.GetConnection().Dispose();
        }


        public async Task<ErrorCode> RegistUserAsync(string authToken, long uid) {

            string key = MemoryDbKeyMaker.MakeUserDataKey(uid.ToString());
            ErrorCode result = ErrorCode.None;

            DaoMdbUserData user = new() {
                AuthToken = authToken,
                UId = uid,
            };

            var expiryTime = TimeSpan.FromMinutes((60 * 3));
            RedisString<DaoMdbUserData> redis = new(_redisConn, key, expiryTime);
            
            if (await redis.SetAsync(user, expiryTime) == false) {
                result = ErrorCode.RedisHasNotKey;
                return result;
            }

            return result;
        }

        public async Task<(ErrorCode, DaoMdbUserData)> GetUserAsync(long uid) {

            var userIDKey = MemoryDbKeyMaker.MakeUserDataKey(uid.ToString());

            RedisString<DaoMdbUserData> redis = new(_redisConn, userIDKey, null);
            RedisResult<DaoMdbUserData> user = await redis.GetAsync();

            if (!user.HasValue) {

                _logger.ZLogError($"[GetUserAsync] UID = {userIDKey}, ErrorMessage = Not Assigned User, RedisString get Error");
                return (ErrorCode.LoginFailUserNotExist, null);
            
            }

            return (ErrorCode.None, user.Value);
        
        }

        public async Task<ErrorCode> RegistClientKeyAsync(long kuid, byte[] clientKey) {

            string key = MemoryDbKeyMaker.MakeClientKeyKey(kuid.ToString());
            ErrorCode result = ErrorCode.None;

            var expiryTime = TimeSpan.FromMinutes((60 * 3));
            RedisString<byte[]> redis = new(_redisConn, key, expiryTime);
            if (await redis.SetAsync(clientKey, expiryTime) == false) {
                result = ErrorCode.RedisHasNotKey;
            }

            return result;
        }

        public async Task<(ErrorCode, byte[])> GetClientKeyAsync(long redisKey) {
            var key = MemoryDbKeyMaker.MakeClientKeyKey(redisKey.ToString());

            RedisString<byte[]> redis = new(_redisConn, key, null);
            RedisResult<byte[]> clientKey = await redis.GetAsync();

            if (!clientKey.HasValue) {
                _logger.ZLogError(
                    $"[GetClientKeyAsync] KUID = {key}, ErrorMessage = Not Assigned Key, RedisString get Error");
                return (ErrorCode.RedisHasNotKey, null);
            }

            return (ErrorCode.None, clientKey.Value);

        }

        public async Task<ErrorCode> DeleteUserAsync(long uid) {

            var userIDKey = MemoryDbKeyMaker.MakeUserDataKey(uid.ToString());

            RedisString<DaoMdbUserData> redis = new(_redisConn, userIDKey, null);
            if (await redis.DeleteAsync() == false) {
                return ErrorCode.RedisHasNotKey;
            }

            return ErrorCode.None;

        }

        public async Task<ErrorCode> UpdateUserAsync(long uid) {

            var userIDKey = MemoryDbKeyMaker.MakeUserDataKey(uid.ToString());
            var key = MemoryDbKeyMaker.MakeClientKeyKey(uid.ToString());

            RedisString<DaoMdbUserData> redisU = new(_redisConn, userIDKey, null);
            if (await redisU.ExpireAsync(TimeSpan.FromMinutes(60 * 3)) == false) {
                return ErrorCode.RedisHasNotKey;
            }

            RedisString<byte[]> redisK = new(_redisConn, key, null);
            if (await redisK.ExpireAsync(TimeSpan.FromMinutes((60 * 3))) == false) {
                return ErrorCode.RedisHasNotKey;
            }

            return ErrorCode.None;


        }


        public async Task<ErrorCode> DeleteClientKeyAsync(long kuid) {

            var key = MemoryDbKeyMaker.MakeClientKeyKey(kuid.ToString());

            RedisString<byte[]> redis = new(_redisConn, key, null);
            if (await redis.DeleteAsync() == false) {
                return ErrorCode.RedisHasNotKey;
            }

            return ErrorCode.None;

        }

        public async Task<ErrorCode> CacheMasterData<T>(string key, List<T> values) {
            
            var masterKey = MemoryDbKeyMaker.MakeMasterCacheKey(key);
            ErrorCode result = ErrorCode.None;

            

            var expiryTime = TimeSpan.FromSeconds(60 * 60 * 24 + Random.Shared.Next(0, 11)); // jitter
            RedisString<List<T>> redis = new(_redisConn, masterKey, expiryTime);
            
            if (await redis.SetAsync(values, expiryTime) == false) {
                result = ErrorCode.RedisHasNotKey;
            }

            return result;

        }

        public async Task<(ErrorCode, List<T>)> GetCashedMasterData<T> (string key) {
            var masterKey = MemoryDbKeyMaker.MakeMasterCacheKey(key);

            RedisString<List<T>> redis = new(_redisConn, masterKey, null);
            RedisResult<List<T>> data = await redis.GetAsync();

            if (!data.HasValue) {

                _logger.ZLogError($"[Get cached master data] key = {masterKey}, ErrorMessage = Not Assigned User, RedisString get Error");
                return (ErrorCode.LoginFailUserNotExist, null);

            }

            return (ErrorCode.None, data.Value);

        }

    }

    public static class MemoryDbKeyMaker {

        public static string MakeUserDataKey(string id) => "UID_" + id;
        public static string MakeClientKeyKey(string kuid) => "KUID_" + kuid;
        public static string MakeMasterCacheKey(string key) => "MASTER_DB_" + key;

    }
}
