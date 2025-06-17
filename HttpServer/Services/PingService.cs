using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.Repository;
using HttpServer.Repository.Interface;
using System.Threading.Tasks;

namespace HttpServer.Services {
    public class PingService : IPingService {

        IMemoryDB_Test _memoryDB;

        public PingService(IMemoryDB_Test memoryDB) {
            _memoryDB = memoryDB;
        }

        public async Task<BasicProtocolRes> UpdateTokenLife(PingReq request) {

            var res = new BasicProtocolRes();

            res.Result = await _memoryDB.UpdateUserAsync(request.UserID);

            return res;

        }

    }

}
