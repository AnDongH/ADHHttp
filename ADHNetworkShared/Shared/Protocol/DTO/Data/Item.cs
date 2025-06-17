using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class ItemInfo {

        public int item_id { get; set; }
        public string item_name { get; set; }
        public int item_type_id { get; set; }
        public int item_rarity_id { get; set; }
        public string item_description { get; set; }

        public ItemInfo(int item_id, string item_name, int item_type_id, int item_rarity_id, string item_description) {
            
            this.item_id = item_id;
            this.item_name = item_name;
            this.item_type_id = item_type_id;
            this.item_rarity_id = item_rarity_id;
            this.item_description = item_description;
        
        }

        public override string ToString() {
            return $"이름: {item_name}\n" +
                   $"등급: {(ItemRarity)item_rarity_id}" +
                   $"타입: {(ItemType)item_type_id}" +
                   $"설명: {item_description}";
        }

    }

    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class RewardInfo {
        
        public List<(ItemInfo, int)> rewardInfo { get; set; }

        public RewardInfo(List<(ItemInfo, int)> rewardInfo) {
            this.rewardInfo = rewardInfo;
        }

    }

    [MemoryPackable]
    public partial class AttendanceRewardInfo : RewardInfo {
    
        public int day_seq { get; set; }

        public AttendanceRewardInfo(int day_seq ,List<(ItemInfo, int)> rewardInfo) : base(rewardInfo) {
            this.day_seq = day_seq;
        }
    
    }

    [MemoryPackable]
    public partial class MailRewardInfo : RewardInfo {

        public int mail_id { get; set; }

        public MailRewardInfo(int mail_id, List<(ItemInfo, int)> rewardInfo) : base(rewardInfo) {
            this.mail_id = mail_id;
        }

    }

    [MemoryPackable]
    public partial class GatchaRewardInfo : RewardInfo {

        public GatchaRewardInfo(List<(ItemInfo, int)> rewardInfo) : base(rewardInfo) {
            
        }

    }

    [MemoryPackable]
    public partial class ItemOtherInfo : ItemInfo {
        public ItemOtherInfo(
            
            int item_id, 
            string item_name, 
            int item_type_id, 
            int item_rarity_id, 
            string item_description
            
            ) : base(item_id, item_name, item_type_id, item_rarity_id, item_description) {
    


        }

        public override string ToString() {
            return base.ToString();
        }

    }

    [MemoryPackable]
    public partial class ItemWeaponInfo : ItemInfo {

        public int attack_damage_min { get; set; }
        public int attack_damage_max { get; set; }
        public int attack_speed { get; set; }

        public ItemWeaponInfo(
            
            int item_id, 
            string item_name, 
            int item_type_id, 
            int item_rarity_id, 
            string item_description, 
            int attack_damage_min, 
            int attack_damage_max, 
            int attack_speed
            
            ) : base(item_id, item_name, item_type_id, item_rarity_id, item_description) {
        
            this.attack_damage_min = attack_damage_min;
            this.attack_damage_max = attack_damage_max;
            this.attack_speed = attack_speed;

        }

        public override string ToString() {
            return base.ToString() +
                   $"최소 데미지: {attack_damage_min}" +
                   $"최대 데미지: {attack_damage_max}" +
                   $"공격 속도: {attack_speed}";
        }

    }

    [MemoryPackable]
    public partial class ItemChestArmorInfo : ItemInfo {
        public int deffence { get; set; }


        public ItemChestArmorInfo(
            
            int item_id, 
            string item_name, 
            int item_type_id, 
            int item_rarity_id, 
            string item_description, 
            int deffence
            
            ) : base(item_id, item_name, item_type_id, item_rarity_id, item_description) {
        
            this.deffence = deffence;

        }

        public override string ToString() {
            return base.ToString() +
                   $"방어력: {deffence}";
        }

    }


    [MemoryPackable]
    public partial class ItemLegArmorInfo : ItemInfo {
        public int deffence { get; set; }

        public ItemLegArmorInfo(

            int item_id,
            string item_name,
            int item_type_id,
            int item_rarity_id,
            string item_description,
            int deffence

        ) : base(item_id, item_name, item_type_id, item_rarity_id, item_description) {

            this.deffence = deffence;

        }

        public override string ToString() {
            return base.ToString() +
                   $"방어력: {deffence}";
        }

    }

    [MemoryPackable]
    public partial class ItemShoesInfo : ItemInfo {
        public int deffence { get; set; }
        public int speed { get; set; }

        public ItemShoesInfo(

            int item_id,
            string item_name,
            int item_type_id,
            int item_rarity_id,
            string item_description,
            int deffence,
            int speed

        ) : base(item_id, item_name, item_type_id, item_rarity_id, item_description) {

            this.deffence = deffence;
            this.speed = speed;

        }

        public override string ToString() {
            return base.ToString() +
                   $"방어력: {deffence}" +
                   $"이동 속도: {speed}";
        }

    }


    [MemoryPackable]
    public partial class ItemHelmetInfo : ItemInfo {
        public int deffence { get; set; }

        public ItemHelmetInfo(

            int item_id,
            string item_name,
            int item_type_id,
            int item_rarity_id,
            string item_description,
            int deffence

        ) : base(item_id, item_name, item_type_id, item_rarity_id, item_description) {

            this.deffence = deffence;

        }

        public override string ToString() {
            return base.ToString() +
                   $"방어력: {deffence}";
        }

    }


    [MemoryPackable]
    public partial class ItemFoodInfo : ItemInfo {
        public int food_value { get; set; }

        public ItemFoodInfo(

            int item_id,
            string item_name,
            int item_type_id,
            int item_rarity_id,
            string item_description,
            int food_value

        ) : base(item_id, item_name, item_type_id, item_rarity_id, item_description) {

            this.food_value = food_value;

        }

        public override string ToString() {
            return base.ToString() +
                   $"효과: {food_value}";
        }

    }

}
