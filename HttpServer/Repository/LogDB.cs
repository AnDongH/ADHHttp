using ADHNetworkShared.Shared.Util;
using HttpServer.DAO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Data;
using System.Threading.Tasks;
using ZLogger;

namespace HttpServer.Repository {
    public class LogDB : ILogDB {

        private IDbConnection _dbConn;
        private readonly DBOptions _dbConfig;
        private readonly MySqlCompiler _compiler;
        private QueryFactory _queryFactory;
        private readonly ILogger<LogDB> _logger;


        public LogDB(ILogger<LogDB> logger, IOptions<DBOptions> dbConfig) {

            _logger = logger;
            _dbConfig = dbConfig.Value;

            // MySql 사용할거임. 이거 말고도 여러가지 컴파일러 지원
            _compiler = new MySqlCompiler();
        }

        public void Dispose() {
            Close();
        }

        private void Open() {
            _dbConn = new MySqlConnection(_dbConfig.LogDB);
            _dbConn.Open();
        }

        private void Close() {
            _dbConn?.Close();
        }

        private bool ActivateConnection() {

            if (_dbConn == null) {

                try {

                    Open();
                    _queryFactory = new QueryFactory(_dbConn, _compiler);
                    return true;

                } catch {

                    return false;

                }

            } else {

                return true;

            }

        }

        public async Task LogToDB(DaoLog log, string table) {

            if (!ActivateConnection()) return;

            int affected = await _queryFactory.Query(table).InsertAsync(log);

            if (affected == 0) {
                _logger.ZLogError($"Error in insert {table} log. please check logDB");
                return;
            }

        }

        public async Task DeleteLog(long uid) {

            if (!ActivateConnection()) return;

            int affected = await _queryFactory
                                .Query("user_account_log")
                                .Where("uid", uid)
                                .DeleteAsync();

            if (affected == 0) {
                _logger.ZLogError($"Error in delete user account log. please check logDB");
                return;
            }

        }

        public async Task CreateLogAccount(long uid) {

            if (!ActivateConnection()) return;

            int affected = await _queryFactory
                                .Query("user_account_log")
                                .InsertAsync(

                                    new {

                                        uid = uid,
                                        create_dt = DateTime.UtcNow

                                    }

                                );

            if (affected == 0) {
                _logger.ZLogError($"Error in create log account. please check logDB");
                return;
            }

        }


    }

}
