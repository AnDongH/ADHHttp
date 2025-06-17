using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HttpServer.Services.Interface {
    public interface IHandShakeService {

        Task SendCommonPublicKey(HttpContext context);

    }
}
