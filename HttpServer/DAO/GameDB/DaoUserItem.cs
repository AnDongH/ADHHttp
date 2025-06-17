using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using HttpServer.Repository;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpServer.DAO {

    public class DaoUserItem {

        public long uid { get; set; }
        public int item_id { get; set; }
        public int cnt { get; set; }

    }

    public class DaoUserItemOther : DaoUserItem {

    }

    public class DaoUserItemWeapon : DaoUserItem {

    }

    public class DaoUserItemChestArmor : DaoUserItem {

    }

    public class DaoUserItemLegArmor : DaoUserItem {

    }

    public class DaoUserItemShoes : DaoUserItem {

    }

    public class DaoUserItemHelmet : DaoUserItem {

    }

    public class DaoUserItemFood : DaoUserItem {

    }

}
