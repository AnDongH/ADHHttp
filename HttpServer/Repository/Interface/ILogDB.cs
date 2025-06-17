using HttpServer.DAO;
using System;
using System.Threading.Tasks;

namespace HttpServer.Repository {
    public interface ILogDB : IDisposable {

        Task LogToDB(DaoLog log, string table);
        Task DeleteLog(long uid);
        Task CreateLogAccount(long uid);
    }
}
