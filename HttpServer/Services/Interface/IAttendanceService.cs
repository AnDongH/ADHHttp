using HttpServer.Controllers;
using ADHNetworkShared.Protocol.DTO;
using ADHNetworkShared.Protocol;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HttpServer.Services {

    public interface IAttendanceService {

        Task<BasicProtocolRes> SetAttendance(DtoAttendanceSetReq request);
        Task<DtoAttendanceGetRes> GetAttendance(DtoAttendanceGetReq request);
        Task<DtoAttendanceCheckRes> CheckAttendanceReward(DtoAttendanceCheckReq request);
        Task<DtoRewardRes> GetReward(DtoAttendanceRewardReq request);

    }

}
