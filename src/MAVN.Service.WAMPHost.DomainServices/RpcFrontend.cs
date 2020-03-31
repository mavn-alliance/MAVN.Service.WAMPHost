using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.WAMPHost.Domain.Services;

namespace MAVN.Service.WAMPHost.DomainServices
{
    [UsedImplicitly]
    public class RpcFrontend : IRpcFrontend
    {
        private readonly IBalanceUpdatePublisher _balanceUpdatePublisher;
        private readonly IWalletStatusUpdatePublisher _walletStatusUpdatePublisher;
        private readonly ISessionCache _sessionCache;

        public RpcFrontend(
            IBalanceUpdatePublisher balanceUpdatePublisher,
            IWalletStatusUpdatePublisher walletStatusUpdatePublisher,
            ISessionCache sessionCache)
        {
            _balanceUpdatePublisher = balanceUpdatePublisher;
            _walletStatusUpdatePublisher = walletStatusUpdatePublisher;
            _sessionCache = sessionCache;
        }

        public async Task InitializeAsync(string topic, string token)
        {
            var customerId = _sessionCache.GetClientId(token);
            
            if (topic == Topic.Balance)
                await _balanceUpdatePublisher.PublishAsync(customerId);
            if (topic == Topic.WalletStatus)
                await _walletStatusUpdatePublisher.PublishAsync(customerId);
        }
    }
}
