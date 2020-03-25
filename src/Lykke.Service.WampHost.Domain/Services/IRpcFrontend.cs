using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using WampSharp.V2.Rpc;

namespace Lykke.Service.WampHost.Domain.Services
{
    public interface IRpcFrontend
    {
        [WampProcedure("init")]
        Task InitializeAsync(string topic, string token);
    }
}