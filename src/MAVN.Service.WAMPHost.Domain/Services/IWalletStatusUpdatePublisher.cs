using System.Threading.Tasks;

namespace MAVN.Service.WAMPHost.Domain.Services
{
    public interface IWalletStatusUpdatePublisher
    {
        void Publish(string customerId, bool isBlocked);

        Task PublishAsync(string customerId);
    }
}
