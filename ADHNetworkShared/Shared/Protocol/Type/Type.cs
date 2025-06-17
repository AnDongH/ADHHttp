using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol {

    public enum ItemType { 
        Other = 0,
        Weapon = 1,
        ChestArmor = 2,
        LegArmor = 3,
        Shoes = 4,
        Helmet = 5,
        Food = 6,
    }

    public enum ItemRarity { 
        None = 0,
        Common = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4,
    }


}
