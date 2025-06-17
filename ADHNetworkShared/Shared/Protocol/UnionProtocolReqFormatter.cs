using ADHNetworkShared.Protocol.DTO;
using MemoryPack;
using MemoryPack.Formatters;


namespace ADHNetworkShared.Protocol {

    [MemoryPackUnionFormatter(typeof(ProtocolReq))]
    [MemoryPackUnion(0, typeof(AuthProtocolReq))]
    [MemoryPackUnion(1, typeof(DtoLoginReq))]
    [MemoryPackUnion(2, typeof(DtoLogoutReq))]
    [MemoryPackUnion(3, typeof(DtoLoginAuthReq))]
    [MemoryPackUnion(4, typeof(DtoAttendanceGetReq))]
    [MemoryPackUnion(5, typeof(DtoAccountRegisterReq))]
    [MemoryPackUnion(6, typeof(DtoAccountDeleteReq))]
    [MemoryPackUnion(7, typeof(PostTestReq))]
    [MemoryPackUnion(8, typeof(DtoHandShakeReq))]
    [MemoryPackUnion(9, typeof(AuthPostTestReq))]
    [MemoryPackUnion(10, typeof(DtoAttendanceSetReq))]
    [MemoryPackUnion(11, typeof(DtoAttendanceRewardReq))]
    [MemoryPackUnion(12, typeof(DtoAttendanceCheckReq))]
    [MemoryPackUnion(13, typeof(PingReq))]
    [MemoryPackUnion(14, typeof(DtoMailListReq))]
    [MemoryPackUnion(15, typeof(DtoMailRewardReq))]
    [MemoryPackUnion(16, typeof(DtoMailDeleteReq))]
    [MemoryPackUnion(17, typeof(DtoItemReq))]
    [MemoryPackUnion(18, typeof(DtoFriendInfoListReq))]
    [MemoryPackUnion(19, typeof(DtoFriendReqInfoListReq))]
    [MemoryPackUnion(20, typeof(DtoFriendReceivedInfoListReq))]
    [MemoryPackUnion(21, typeof(DtoFriendReqReq))]
    [MemoryPackUnion(22, typeof(DtoFriendAcceptReq))]
    [MemoryPackUnion(23, typeof(DtoFriendDeleteReq))]
    [MemoryPackUnion(24, typeof(DtoFriendReqCancelReq))]
    [MemoryPackUnion(25, typeof(DtoFriendReqDenyReq))]
    [MemoryPackUnion(26, typeof(DtoAccountInfoReq))]
    [MemoryPackUnion(27, typeof(DtoScoreSetReq))]
    [MemoryPackUnion(28, typeof(DtoGetMyRankingReq))]
    [MemoryPackUnion(29, typeof(DtoGetAllRankingReq))]
    [MemoryPackUnion(30, typeof(ItemDBTestReq))]
    [MemoryPackUnion(31, typeof(ItemDBUpdateTestReq))]
    [MemoryPackUnion(32, typeof(DtoGatchaewardReq))]
    public partial class UnionProtocolReqFormatter {

    }

    [MemoryPackUnionFormatter(typeof(ProtocolRes))]
    [MemoryPackUnion(0, typeof(DtoLoginRes))]
    [MemoryPackUnion(1, typeof(BasicProtocolRes))]
    [MemoryPackUnion(2, typeof(DtoAttendanceGetRes))]
    [MemoryPackUnion(3, typeof(PostTestRes))]
    [MemoryPackUnion(4, typeof(DtoHandShakeRes))]
    [MemoryPackUnion(5, typeof(DtoRewardRes))]
    [MemoryPackUnion(6, typeof(DtoAttendanceCheckRes))]
    [MemoryPackUnion(7, typeof(DtoMailListRes))]
    [MemoryPackUnion(8, typeof(DtoUserItemInfosRes))]
    [MemoryPackUnion(9, typeof(DtoFriendInfoListRes))]
    [MemoryPackUnion(10, typeof(DtoGetMyRankingRes))]
    [MemoryPackUnion(11, typeof(DtoGetAllRankingRes))]
    [MemoryPackUnion(12, typeof(ItemDBTestRes))]
    public partial class UnionProtocolResFormatter {

    }

    [MemoryPackUnionFormatter(typeof(ItemInfo))]
    [MemoryPackUnion(0, typeof(ItemOtherInfo))]
    [MemoryPackUnion(1, typeof(ItemWeaponInfo))]
    [MemoryPackUnion(2, typeof(ItemChestArmorInfo))]
    [MemoryPackUnion(3, typeof(ItemLegArmorInfo))]
    [MemoryPackUnion(4, typeof(ItemShoesInfo))]
    [MemoryPackUnion(5, typeof(ItemHelmetInfo))]
    [MemoryPackUnion(6, typeof(ItemFoodInfo))]
    public partial class UnionItemInfoFormatter {

    }

    [MemoryPackUnionFormatter(typeof(RewardInfo))]
    [MemoryPackUnion(0, typeof(AttendanceRewardInfo))]
    [MemoryPackUnion(1, typeof(MailRewardInfo))]
    [MemoryPackUnion(2, typeof(GatchaRewardInfo))]
    public partial class UnionRewardInfoFormatter {

    }

}
