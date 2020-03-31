using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.PrivateBlockchainFacade.Contract.Events;
using MAVN.Service.WAMPHost.Domain.Services;

namespace MAVN.Service.WAMPHost.DomainServices.Subscribers
{
    [UsedImplicitly]
    public class BalanceUpdatedEventSubscriber
        : RabbitSubscriber<CustomerBalanceUpdatedEvent>
    {
        private readonly IBalanceUpdatePublisher _balanceUpdatePublisher;
        
        public BalanceUpdatedEventSubscriber(
            string connectionString,
            string exchangeName,
            ILogFactory logFactory,
            IBalanceUpdatePublisher balanceUpdatePublisher)
                : base(connectionString, exchangeName, logFactory)
        {
            _balanceUpdatePublisher = balanceUpdatePublisher;
        }

        protected override async Task<(bool isSuccessful, string errorMessage)> ProcessMessageAsync(CustomerBalanceUpdatedEvent message)
        {
            await _balanceUpdatePublisher.PublishAsync(message.CustomerId, message.Reason.ToString(), message.Balance, message.Timestamp);

            return (true, null);
        }
    }
}
