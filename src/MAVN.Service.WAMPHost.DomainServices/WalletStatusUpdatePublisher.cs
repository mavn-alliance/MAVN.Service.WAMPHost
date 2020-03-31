using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.WalletManagement.Client;
using Lykke.Service.WalletManagement.Client.Enums;
using MAVN.Service.WAMPHost.Contract;
using MAVN.Service.WAMPHost.Domain.Services;
using WampSharp.V2;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Realm;

namespace MAVN.Service.WAMPHost.DomainServices
{
    public class WalletStatusUpdatePublisher : IWalletStatusUpdatePublisher
    {
        private readonly IWampHostedRealm _realm;
        private readonly IWalletManagementClient _walletManagementClient;
        private readonly ISessionCache _sessionCache;
        private readonly ILog _log;

        public WalletStatusUpdatePublisher(
            IWampHostedRealm realm,
            IWalletManagementClient walletManagementClient,
            ISessionCache sessionCache,
            ILogFactory logFactory)
        {
            _realm = realm;
            _walletManagementClient = walletManagementClient;
            _sessionCache = sessionCache;
            _log = logFactory.CreateLog(this);
        }

        public void Publish(string customerId, bool isBlocked)
        {
            var sessionIds = _sessionCache.GetSessionIds(customerId);

            if (sessionIds.Length == 0)
                return;

            var subject = _realm.Services.GetSubject(Topic.WalletStatus);

            subject.OnNext(new WampEvent
            {
                Options = new PublishOptions {Eligible = sessionIds},
                Arguments = new object[] {new WalletStatusUpdate {IsBlocked = isBlocked}}
            });
        }

        public async Task PublishAsync(string customerId)
        {
            var response = await _walletManagementClient.Api.GetCustomerWalletBlockStateAsync(customerId);

            if (response.Error != CustomerWalletBlockStatusError.None)
            {
                _log.Warning("An error occurred while getting customer wallet state.",
                    context: $"customerId: {customerId}; error: {response.Error}");
                return;
            }

            var isBlocked = response.Status == CustomerWalletActivityStatus.Blocked;

            Publish(customerId, isBlocked);
        }
    }
}
