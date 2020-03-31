using System;
using System.Threading.Tasks;
using Falcon.Numerics;
using Moq;
using Xunit;

namespace MAVN.Service.WAMPHost.Tests.DomainServices
{
    public class RpcFrontendTests
    {
        private const string BalanceUpdateTopic = "balance";
        
        [Fact]
        public async Task Sends_Init_Balance()
        {
            // Arrange
            var fixture = new RpcFrontendTestsFixture();
            
            // Act
            await fixture.Service.InitializeAsync(BalanceUpdateTopic, Guid.NewGuid().ToString());
            
            // Assert
            fixture.BalanceUpdatePublisherMock.Verify( x => x.PublishAsync(
                It.Is<string>(c => c == fixture.CustomerId),
                It.Is<string>(c => c == null),
                It.Is<Money18?>(c => c == null),
                It.Is<DateTime?>(c => c == null)),
                Times.Once);
        }
        
        [Fact]
        public async Task Does_Not_Send_When_Unknown_Topic()
        {
            // Arrange
            var fixture = new RpcFrontendTestsFixture();
            
            // Act
            await fixture.Service.InitializeAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            
            // Assert
            fixture.BalanceUpdatePublisherMock.Verify( x => x.PublishAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Money18?>(),
                    It.IsAny<DateTime?>()),
                Times.Never);
        }
    }
}