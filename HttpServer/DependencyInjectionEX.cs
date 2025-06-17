using ADHNetworkShared.Shared.Util;
using HttpServer.Repository;
using HttpServer.Repository.Interface;
using HttpServer.Services;
using HttpServer.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace HttpServer;

public static class DependencyInjectionEX
{
    public static IServiceCollection AddMyServices(this IServiceCollection services)
    {
        services.AddTransient<IGameDB, GameDB>();
        services.AddTransient<ILogDB, LogDB>();
        //services.AddSingleton<IMemoryDB, MemoryDB>();
        services.AddSingleton<IMemoryDB_Test, MemoryDB_Test>();
        
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IAttendanceService, AttendanceService>();
        services.AddTransient<ILoginAuthenticationService, LoginAuthenticationService>();
        services.AddTransient<ILoginService, LoginService>();
        services.AddTransient<IDataProcessService, DataProcessService>();
        services.AddTransient<ITestService, TestService>();
        services.AddTransient<IHandShakeService, HandShakeService>();
        services.AddTransient<IMailService, MailService>();
        services.AddTransient<IItemService, ItemService>();
        services.AddTransient<IPingService, PingService>();
        services.AddTransient<IFriendService, FriendService>();
        services.AddTransient<IRankingService, RankingService>();
        
        return services;
    }

    public static IServiceCollection AddConfig(this IServiceCollection services)
    {
        
        services.Configure<DBOptions>(ConfigData._config.GetSection(DBOptions.DbConfig));
        services.Configure<KeyOption>(ConfigData._config.GetSection(KeyOption.CommonKey));
        
        return services;
    }
}