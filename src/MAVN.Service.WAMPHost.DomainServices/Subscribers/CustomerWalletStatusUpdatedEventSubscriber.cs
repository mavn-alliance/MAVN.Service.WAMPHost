using System.Threading.Tasks;
using Lykke.Common.Log;
using MAVN.Service.WalletManagement.Contract.Events;
using MAVN.Service.WAMPHost.Domain.Services;

namespace MAVN.Service.WAMPHost.DomainServices.Subscribers
{
    public class CustomerWalletStatusUpdatedEventSubscriber : RabbitSubscriber<CustomerWalletStatusUpdatedEvent>
    {
        private readonly IWalletStatusUpdatePublisher _walletStatusUpdatePublisher;

        public CustomerWalletStatusUpdatedEventSubscriber(
            string connectionString,
            string exchangeName,
            IWalletStatusUpdatePublisher walletStatusUpdatePublisher,
            ILogFactory logFactory)
            : base(connectionString, exchangeName, logFactory)
        {
            _walletStatusUpdatePublisher = walletStatusUpdatePublisher;
        }

        protected override Task<(bool isSuccessful, string errorMessage)> ProcessMessageAsync(
            CustomerWalletStatusUpdatedEvent message)
        {
            _walletStatusUpdatePublisher.Publish(message.CustomerId, message.WalletBlocked);

            return Task.FromResult<(bool isSuccessful, string errorMessage)>((true, null));
        }
    }
}
