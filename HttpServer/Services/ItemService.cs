using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.DAO;
using HttpServer.Repository;
using MemoryPack;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpServer.Services {

    public class ItemService : IItemService {

        private readonly IGameDB _gameDB;

        public ItemService(IGameDB gameDB) {

            _gameDB = gameDB;

        }

        public async Task<DtoUserItemInfosRes> GetItemList(DtoItemReq request) {
            
            var res = new DtoUserItemInfosRes();

            if (_gameDB.ActivateConnection()) {

                (res.Result, res.userItemInfos) = await _gameDB.GetUserItemList(Router.UserItemTableMap[request.itemType], request.UserID);

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;
            
        }

        public async Task<DtoRewardRes> GetGatchaReward(DtoGatchaewardReq request) {

            var res = new DtoRewardRes();

            if (_gameDB.ActivateConnection()) {

                var probData = await _gameDB.GetMasterDB<DaoMasterDBGatchaProb>("master_gatcha_prob");
                var cntData = await _gameDB.GetMasterDB<DaoMasterDBGatchaReward>("master_gatcha_reward");
                var masterItemData = await _gameDB.GetMasterItemInfos(true);

                List<int> probList = new List<int>(100);
                
                foreach (var data in probData) {

                    for (int i = 0; i < data.item_prob; i++) {
                        probList.Add(data.item_rarity_id);
                    }
                
                }

                SecureRandom r = new();
                res.rewardInfos = new GatchaRewardInfo(new List<(ItemInfo, int)>());

                for (int i = 0; i < 10; i++) {


                    // var items = masterItemData.Where(x => x.item_rarity_id == probList[Random.Shared.Next(0, probList.Count)]).ToList();
                    // int rand = Random.Shared.Next(0, items.Count);

                    // ItemInfo item = items[rand];
                    // res.rewardInfos.rewardInfo.Add((item, cntData.Find(x => x.item_id == item.item_id).cnt));

                    // 지연 평가를 하는 메서드의 람다식을 인자로 넣을 때 랜덤값을 넣으면 안된다..
                    // => 실제로 열거자를 순회하는 순간에 그 순간마다 랜덤식이 실행되어서 의도된 값이 안나오고, 
                    // 중복해서 열거자를 순회하는 순간 순회되는 값이 달라진다.
                    // 따라서 이렇게 미리 값을 캐싱해서 사용하거나, ToList를 통해 items값을 고정시키고 값을 뽑는 것이 좋다.
                    int randID = probList[r.Next(0, probList.Count)];
                    var items = masterItemData.Where(x => x.item_rarity_id == randID);
                    ItemInfo item = items.ElementAt(r.Next(0, items.Count()));
                    res.rewardInfos.rewardInfo.Add((item, cntData.Where(x => x.item_id == item.item_id).FirstOrDefault().item_cnt));

                }

                res.Result = await _gameDB.UpdateGatchaRewardInfo(res.rewardInfos as GatchaRewardInfo, request.UserID);

                if (res.Result != ErrorCode.None) res.rewardInfos = null;

            } else {

                res.Result = ErrorCode.DBConnectionFailException;

            }

            return res;

        }

    }

}
