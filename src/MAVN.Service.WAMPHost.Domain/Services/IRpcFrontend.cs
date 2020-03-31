using System.Threading.Tasks;
using WampSharp.V2.Rpc;

namespace MAVN.Service.WAMPHost.Domain.Services
{
    public interface IRpcFrontend
    {
        [WampProcedure("init")]
        Task InitializeAsync(string topic, string token);
    }
}