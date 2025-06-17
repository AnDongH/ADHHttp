using System;
using System.Collections.Generic;
using ADHNetworkShared.Protocol.DTO;

namespace ADHNetworkShared.Protocol {
    public static class Router {

        private static Dictionary<ProtocolID, string> routingMap = new Dictionary<ProtocolID, string>()
        {   
            { ProtocolID.Handshake, "/noneAuth/handshake" },
            { ProtocolID.PostTest, "/noneAuth/posttest" },
            { ProtocolID.AuthPostTest, "/test/authposttest" },
            { ProtocolID.CreateAccount, "/noneAuth/account/create"},
            { ProtocolID.DeleteAccount, "/account/delete"},
            { ProtocolID.UserInfo, "/account/info"},
            { ProtocolID.Login, "/noneAuth/login"},
            { ProtocolID.Logout, "/logout"},
            { ProtocolID.GetAttendance, "/attendance/getinfo"}, 
            { ProtocolID.SetAttendance, "/attendance/setinfo"}, 
            { ProtocolID.CheckAttendance, "/attendance/check"}, 
            { ProtocolID.RewardAttendance, "/attendance/reward"}, 
            { ProtocolID.Ping, "/ping/ping"}, 
            { ProtocolID.Mail, "/mail/get"},
            { ProtocolID.RewardMail, "/mail/reward"}, 
            { ProtocolID.DeleteMail, "/mail/delete"}, 
            { ProtocolID.FriendInfo, "/friend/info"},
            { ProtocolID.FriendReqInfo, "/friend/reqinfo"},
            { ProtocolID.FriendRcvInfo, "/friend/rcvinfo"},
            { ProtocolID.FriendReq, "/friend/req"},
            { ProtocolID.FriendAccept, "/friend/acc"},
            { ProtocolID.FriendDelete, "/friend/del"},
            { ProtocolID.FriendCancel, "/friend/cancel"},
            { ProtocolID.FriendDeny, "/friend/deny"},
            { ProtocolID.SetScore, "/score/set"},
            { ProtocolID.GetMyRanking, "/score/get/my"},
            { ProtocolID.GetAllRanking, "/score/get/all"},
            { ProtocolID.Item, "/item/list"},
            { ProtocolID.ItemDBTest, "/test/itemlist"},
            { ProtocolID.ItemDBUpdateTest, "/test/updateitemlist"},
            { ProtocolID.Gatcha, "/item/gatcha"},
            

        };

        private static Dictionary<ItemType, string> masterItemTableMap = new Dictionary<ItemType, string>() {

            { ItemType.Other, "master_item_other"},
            { ItemType.Weapon, "master_item_weapon"},
            { ItemType.ChestArmor, "master_item_chest_armor"},
            { ItemType.LegArmor, "master_item_leg_armor"},
            { ItemType.Shoes, "master_item_shoes"},
            { ItemType.Helmet, "master_item_helmet"},
            { ItemType.Food, "master_item_food"},
        
        };

        private static Dictionary<ItemType, string> userItemTableMap = new Dictionary<ItemType, string>() {

            { ItemType.Other, "user_item_other"},
            { ItemType.Weapon, "user_item_weapon"},
            { ItemType.ChestArmor, "user_item_chest_armor"},
            { ItemType.LegArmor, "user_item_leg_armor"},
            { ItemType.Shoes, "user_item_shoes"},
            { ItemType.Helmet, "user_item_helmet"},
            { ItemType.Food, "user_item_food"},

        };

        public static Dictionary<ProtocolID, string> RoutingMap => routingMap;
        public static Dictionary<ItemType, string> MasterItemTableMap => masterItemTableMap;
        public static Dictionary<ItemType, string> UserItemTableMap => userItemTableMap;
        
    }

}
