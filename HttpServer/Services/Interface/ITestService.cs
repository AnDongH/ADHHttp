using ADHNetworkShared.Protocol.DTO;
using System.Threading.Tasks;

namespace HttpServer.Services {
    public interface ITestService {
        
        PostTestRes PostTest(PostTestReq request);
        PostTestRes AuthPostTest(AuthPostTestReq request);
        Task<ItemDBTestRes> ItemDBTest(ItemDBTestReq request);
        Task<BasicProtocolRes> ItemDBUpdateTest(ItemDBUpdateTestReq request);
    }
}
