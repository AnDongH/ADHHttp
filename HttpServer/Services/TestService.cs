using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace HttpServer.Services {
    public class TestService : ITestService {

        private readonly IGameDB _gameDB;

        public TestService(IGameDB gameDB) {
            _gameDB = gameDB;
        }

        public PostTestRes PostTest(PostTestReq request) {

            return new PostTestRes(request.requestMSG + " - Server Received!!");

        }

        public PostTestRes AuthPostTest(AuthPostTestReq request) {

            return new PostTestRes(request.requestMSG + " - Server Received!!");

        }

        public async Task<ItemDBTestRes> ItemDBTest(ItemDBTestReq request) {

            ItemDBTestRes res = new ItemDBTestRes();

            if (_gameDB.ActivateConnection()) {

                res.infos = await _gameDB.GetMasterItemInfos(request.isCache);

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;

        }

        public async Task<BasicProtocolRes> ItemDBUpdateTest(ItemDBUpdateTestReq request) {

            BasicProtocolRes res = new BasicProtocolRes();

            if (_gameDB.ActivateConnection()) {

                var masteritemList = await _gameDB.GetMasterItemInfos(true);

                foreach (var updateInfo in request.updateInfo) {

                    ItemType type = (ItemType)masteritemList.Find(x => x.item_id == updateInfo.Item1).item_type_id;
                    string table = Router.UserItemTableMap[type];

                    res.Result = await _gameDB.UpdateUserItem(table, null, request.UserID, updateInfo.Item1, updateInfo.Item2);
                    if (res.Result != ErrorCode.None) return res;
                
                }

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;

        }

    }


}
