using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.Repository;
using HttpServer.Repository.Interface;
using MemoryPack;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HttpServer.DAO {

    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class DaoMasterDBItem {
        public int item_id { get; set; }
        public string item_name { get; set; }
        public int item_type_id { get; set; }
        public int item_rarity_id { get; set; }
        public string item_description { get; set; }

        public virtual ItemInfo GetItemInfo() {
            return new ItemOtherInfo(item_id, item_name, item_type_id, item_rarity_id, item_description);
        }

    }

    public class DaoMasterDBItemCacheMaker : CacheMaker {
        public DaoMasterDBItemCacheMaker(string key) : base(key) {
        }

        public override async Task<bool> MakeCacheData(QueryFactory _queryFactory, IMemoryDB_Test _memoryDB, IDbTransaction transaction = null) {

            var datas = new List<DaoMasterDBItem>();

            datas.AddRange(await _queryFactory.Query("master_item").Join("master_item_other", "master_item.item_id", "master_item_other.item_id").GetAsync<DaoMasterDBItemOther>());
            datas.AddRange(await _queryFactory.Query("master_item").Join("master_item_weapon", "master_item.item_id", "master_item_weapon.item_id").GetAsync<DaoMasterDBItemWeapon>());
            datas.AddRange(await _queryFactory.Query("master_item").Join("master_item_chest_armor", "master_item.item_id", "master_item_chest_armor.item_id").GetAsync<DaoMasterDBItemChestArmor>());
            datas.AddRange(await _queryFactory.Query("master_item").Join("master_item_leg_armor", "master_item.item_id", "master_item_leg_armor.item_id").GetAsync<DaoMasterDBItemLegArmor>());
            datas.AddRange(await _queryFactory.Query("master_item").Join("master_item_shoes", "master_item.item_id", "master_item_shoes.item_id").GetAsync<DaoMasterDBItemShoes>());
            datas.AddRange(await _queryFactory.Query("master_item").Join("master_item_helmet", "master_item.item_id", "master_item_helmet.item_id").GetAsync<DaoMasterDBItemHelmet>());
            datas.AddRange(await _queryFactory.Query("master_item").Join("master_item_food", "master_item.item_id", "master_item_food.item_id").GetAsync<DaoMasterDBItemFood>());

            if ((await _memoryDB.CacheMasterData(key, datas)) != ErrorCode.None) return false;

            return true;

        }
    
    }

    [MemoryPackable]
    public partial class DaoMasterDBItemOther : DaoMasterDBItem {
        public override ItemInfo GetItemInfo() {
            return new ItemOtherInfo(item_id, item_name, item_type_id, item_rarity_id, item_description);
        }
    
    }

    [MemoryPackable]
    public partial class DaoMasterDBItemWeapon : DaoMasterDBItem {

        public int attack_damage_min { get; set; }
        public int attack_damage_max { get; set; }
        public int attack_speed { get; set; }

        public override ItemInfo GetItemInfo() {
            return new ItemWeaponInfo(item_id, item_name, item_type_id, item_rarity_id, item_description, attack_damage_min, attack_damage_max, attack_speed);
        }
    }

    [MemoryPackable]
    public partial class DaoMasterDBItemChestArmor : DaoMasterDBItem {
        public int deffence { get; set; }

        public override ItemInfo GetItemInfo() {
            return new ItemChestArmorInfo(item_id, item_name, item_type_id, item_rarity_id, item_description, deffence);
        }
    }

    [MemoryPackable]
    public partial class DaoMasterDBItemLegArmor : DaoMasterDBItem {
        public int deffence { get; set; }

        public override ItemInfo GetItemInfo() {
            return new ItemLegArmorInfo(item_id, item_name, item_type_id, item_rarity_id, item_description, deffence);
        }
    }

    [MemoryPackable]
    public partial class DaoMasterDBItemShoes : DaoMasterDBItem {
        public int deffence { get; set; }
        public int speed { get; set; }

        public override ItemInfo GetItemInfo() {
            return new ItemShoesInfo(item_id, item_name, item_type_id, item_rarity_id, item_description, deffence, speed);
        }
    }

    [MemoryPackable]
    public partial class DaoMasterDBItemHelmet : DaoMasterDBItem {
        public int deffence { get; set; }

        public override ItemInfo GetItemInfo() {
            return new ItemHelmetInfo(item_id, item_name, item_type_id, item_rarity_id, item_description, deffence);
        }
    }

    [MemoryPackable]
    public partial class DaoMasterDBItemFood : DaoMasterDBItem {
        public int food_value { get; set; }

        public override ItemInfo GetItemInfo() {
            return new ItemFoodInfo(item_id, item_name, item_type_id, item_rarity_id, item_description, food_value);
        }
    }

}
