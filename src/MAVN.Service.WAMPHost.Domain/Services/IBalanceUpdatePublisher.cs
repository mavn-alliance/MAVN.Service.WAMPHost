using System;
using System.Threading.Tasks;
using MAVN.Numerics;

namespace MAVN.Service.WAMPHost.Domain.Services
{
    public interface IBalanceUpdatePublisher
    {
        Task PublishAsync(string customerId, string reason = default(string), Money18? balance = default(Money18?), DateTime? timestamp = default(DateTime?));
    }
}
