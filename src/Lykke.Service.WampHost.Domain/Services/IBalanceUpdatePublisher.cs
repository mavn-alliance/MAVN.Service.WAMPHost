using System;
using System.Threading.Tasks;
using Falcon.Numerics;

namespace Lykke.Service.WampHost.Domain.Services
{
    public interface IBalanceUpdatePublisher
    {
        Task PublishAsync(string customerId, string reason = default(string), Money18? balance = default(Money18?), DateTime? timestamp = default(DateTime?));
    }
}