using System.Threading.Tasks;

namespace Lykke.Service.WampHost.Domain.Services
{
    public interface IWalletStatusUpdatePublisher
    {
        void Publish(string customerId, bool isBlocked);

        Task PublishAsync(string customerId);
    }
}
