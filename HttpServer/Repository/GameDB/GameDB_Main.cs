using Microsoft.Extensions.Options;
using System.Data;
using System.Threading.Tasks;
using System;
using MySqlConnector;
using SqlKata.Execution;
using SqlKata.Compilers;
using Microsoft.Extensions.Logging;
using ZLogger;
using HttpServer.DAO;
using System.Text;
using ADHNetworkShared.Protocol;
using System.Collections.Generic;
using System.Linq;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.Repository.Interface;

namespace HttpServer.Repository {

    public partial class GameDB : IGameDB {

        // IOptions<구성 설정 클래스> 로 가져오고 생성자에 넣어주면 매핑한 설정 가져올 수 있음(의존성 주입 - 생성자 주입으로 인해)
        private readonly DBOptions _dbConfig;

        // sql연결을 관리해주는 인터페이스
        private IDbConnection _dbConn;
        
        // sql 구문을 mySQL에 맞춰서 변경해주는 놈
        private readonly MySqlCompiler _compiler;
        
        // db 연결 관리. sql 작성할 수 있도록 함. 실행 후 결과 반환
        private QueryFactory _queryFactory;

        private ILogger<GameDB> _logger;


        private readonly IMemoryDB_Test _memoryDB;
        private readonly ILogDB _logDB;

        public GameDB(ILogger<GameDB> logger,IOptions<DBOptions> dbConfig, IMemoryDB_Test memoryDB, ILogDB logDB) {

            // 의존성 주입
            _dbConfig = dbConfig.Value;
            _logger = logger;
            _memoryDB = memoryDB;
            _logDB = logDB;
            
            // MySql 사용할거임. 이거 말고도 여러가지 컴파일러 지원
            _compiler = new MySqlCompiler();
        }

        public void Dispose() {
            Close(); 
        }

        // db 연결.
        private void Open() {
            _dbConn = new MySqlConnection(_dbConfig.GameDB);
            _dbConn.Open();
        }

        private void Close() {
            _dbConn?.Close();
        }

        public bool ActivateConnection() {

            if (_dbConn == null) {

                try {

                    Open();
                    _queryFactory = new QueryFactory(_dbConn, _compiler);
                    return true;

                } catch (Exception ex){

                    _logger.LogError(ex.ToString());
                    return false;
                
                }

            } else {

                return true;

            }

        }


    }

    // db 구성. appsettings.json에 있는 구성이 종속성 컨테이너로 인해서 자동으로 파싱
    public class DBOptions {

        public const string DbConfig = "DbConfig";

        public string GameDB { get; set; }
        public string LogDB { get; set; }
        public string Redis { get; set; }
   
    }
}
