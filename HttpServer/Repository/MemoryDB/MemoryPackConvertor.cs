using CloudStructures.Converters;
using HttpServer.DAO;
using MemoryPack;

namespace HttpServer.Repository {
    public class MemoryPackConvertor : IValueConverter {
        public T Deserialize<T>(byte[] value) {
            return MemoryPackSerializer.Deserialize<T>(value);
        }

        public byte[] Serialize<T>(T value) {
            return MemoryPackSerializer.Serialize(value);
        }
    }

    [MemoryPackUnionFormatter(typeof(DaoMasterDBItem))]
    [MemoryPackUnion(0, typeof(DaoMasterDBItemOther))]
    [MemoryPackUnion(1, typeof(DaoMasterDBItemWeapon))]
    [MemoryPackUnion(2, typeof(DaoMasterDBItemChestArmor))]
    [MemoryPackUnion(3, typeof(DaoMasterDBItemLegArmor))]
    [MemoryPackUnion(4, typeof(DaoMasterDBItemShoes))]
    [MemoryPackUnion(5, typeof(DaoMasterDBItemHelmet))]
    [MemoryPackUnion(6, typeof(DaoMasterDBItemFood))]
    public partial class UnionMasterDBItemFormatter {

    }

    [MemoryPackUnionFormatter(typeof(DaoMasterDBReward))]
    [MemoryPackUnion(1, typeof(DaoMasterDBAttendanceReward))]
    [MemoryPackUnion(2, typeof(DaoMasterDBMailReward))]
    public partial class UnionMasterDBRewardFormatter {

    }

}
