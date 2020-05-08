using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using MAVN.Numerics;
using MAVN.Service.WAMPHost.Contract;
using Moq;
using WampSharp.V2;
using Xunit;

namespace MAVN.Service.WAMPHost.Tests.DomainServices
{
    public class BalanceUpdatePublisherTests
    {
        [Fact]
        public async Task Publishes_BalanceUpdate()
        {
            // Arrange
            var fixture = new BalanceUpdatePublisherTestsFixture();
            var balance = new Money18(new BigInteger(234), 0);
            
            // Act
            await fixture.Service.PublishAsync(fixture.CustomerId, null, balance, DateTime.UtcNow);
            
            // Assert
            fixture.BlockchainFacadeClientMock.Verify(x => x.CustomersApi.GetBalanceAsync(It.IsAny<Guid>()),
                Times.Never);
            
            fixture.WampSubjectMock.Verify(x => x.OnNext(It.Is<IWampEvent>(
                c =>
                    c.Options.Eligible.Contains(fixture.SessionId) &&
                    (c.Arguments[0] as BalanceUpdate).Balance == balance.ToString("N0", 2, new CultureInfo("en-US").NumberFormat))),
                Times.Once);
        }
        
        [Fact]
        public async Task Publishes_BalanceUpdate_After_RestRequest()
        {
            // Arrange
            var fixture = new BalanceUpdatePublisherTestsFixture();
            
            // Act
            await fixture.Service.PublishAsync(fixture.CustomerId);
            
            // Assert
            fixture.BlockchainFacadeClientMock.Verify(
                x => x.CustomersApi.GetBalanceAsync(It.Is<Guid>(g => g.ToString() == fixture.CustomerId)),
                Times.Once);
            
            fixture.WampSubjectMock.Verify(x => x.OnNext(It.Is<IWampEvent>(
                    c =>
                        c.Options.Eligible.Contains(fixture.SessionId) &&
                        (c.Arguments[0] as BalanceUpdate).Balance == ((Money18)fixture.BalanceFromService).ToString("N0", 2, new CultureInfo("en-US").NumberFormat))),
                Times.Once);
        }
    }
}
