using ADHNetworkShared.Protocol.DTO;
using HttpServer.DAO;
using ADHNetworkShared.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using HttpServer.Repository;
using Microsoft.Extensions.DependencyInjection;
using HttpServer.Services;
using HttpServer.Middleware;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using ZLogger;
using Microsoft.Extensions.Configuration;
using ADHNetworkShared.Shared.Util;
using HttpServer.Services.Interface;
using Microsoft.AspNetCore.Http.Timeouts;
using System.Threading.Tasks;
using System.Net.Http;
using HttpServer.Repository.Interface;



namespace HttpServer {
    public class Program {

        public static async Task Main(string[] args) {
            
            // 1. 기본 설정 초기화
            var builder = WebApplication.CreateBuilder(args);

            // 이거 안해도 그냥 종속성 주입으로 사용 가능하긴 함 -> 커스텀 설정을 위해 사용하는거
            Config.Load("appsettings.json", AppContext.BaseDirectory);
            Config.Load($"appsettings.{builder.Environment.EnvironmentName}.json", AppContext.BaseDirectory);

            // 2. 컨테이너에 서비스 등록
            builder.Services.AddConfig();
            builder.Services.AddMyServices();
            builder.Services.AddRequestTimeouts();
            builder.Services.AddControllers();
            builder.Services.AddLogging();

            SettingLogger(builder);

            // 3. 앱 빌드
            var app = builder.Build();

            // 4. 파이프라인 구성
            // ������ DB �ʱ�ȭ
            if (!await app.Services.GetService<IGameDB>().LoadMasterData()) {
                return;
            }


            app.UseRequestTimeouts();

            app.UseReqPerSecMiddleware();

            app.UseWhen(context => !context.Request.Path.StartsWithSegments("/noneAuth", StringComparison.OrdinalIgnoreCase), appBuilder => {
                appBuilder.UseMiddleware<AuthReqMiddleware>();
            });

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/noneAuth", StringComparison.OrdinalIgnoreCase) && !context.Request.Path.Value.Contains("handshake"), appBuilder => {
                appBuilder.UseMiddleware<NoneAuthReqMiddleware>();
            });
            
            app.MapControllers();

            // 5. 후 설정
            UnionProtocolReqFormatterInitializer.RegisterFormatter();
            UnionProtocolResFormatterInitializer.RegisterFormatter();
            UnionItemInfoFormatterInitializer.RegisterFormatter();
            UnionRewardInfoFormatterInitializer.RegisterFormatter();
            UnionMasterDBItemFormatterInitializer.RegisterFormatter();
            UnionMasterDBRewardFormatterInitializer.RegisterFormatter();

            // 6. 서버 실행
            app.Run(ConfigData._config["ServerUrl"]);
        
        }

        static void SettingLogger(WebApplicationBuilder builder) {

            // ILoggingBuilder -> �α׿� ���� ������ ������ �� �ִ� �������̽�
            ILoggingBuilder logging = builder.Logging;
            _ = logging.ClearProviders();

            logging.AddConfiguration(ConfigData._config);

            ///////////////////////// ���� �׽�Ʈ�� ///////////////////////////
            logging.SetMinimumLevel(LogLevel.Warning);
            ///////////////////////////////////////////////////////////////////
            
            string fileDir = ConfigData._config["logdir"];

            if (fileDir == null) {
                throw new Exception("logdir is not set in appsettings.json");
            }

            bool exists = Directory.Exists(fileDir);

            if (!exists) {
                _ = Directory.CreateDirectory(fileDir);
            }


            // �α� ���� ���� �Ǵ� ����
            _ = logging.AddZLoggerRollingFile(
                options => {

                    options.UseFormatter(() => new LogFormatter());
                    options.FilePathSelector = (timestamp, sequenceNumber) => $"{fileDir}{timestamp.ToLocalTime():yyyy-MM-dd}_{sequenceNumber:000}.log";
                    options.RollingInterval = ZLogger.Providers.RollingInterval.Day;
                    options.RollingSizeKB = 1024;

                });

            // �α׸� �ֿܼ��� ���
            _ = logging.AddZLoggerConsole(options => {
                options.UseFormatter(() => new LogFormatter());
            });

        }
    }
}
