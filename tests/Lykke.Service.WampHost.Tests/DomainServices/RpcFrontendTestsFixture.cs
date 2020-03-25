using System;
using System.Threading.Tasks;
using Falcon.Numerics;
using Lykke.Service.WampHost.Domain.Services;
using Lykke.Service.WampHost.DomainServices;
using Moq;

namespace Lykke.Service.WampHost.Tests.DomainServices
{
    public class RpcFrontendTestsFixture
    {
        public Mock<ISessionCache> SessionCacheMock { get; }

        public Mock<IBalanceUpdatePublisher> BalanceUpdatePublisherMock { get; }

        public Mock<IWalletStatusUpdatePublisher> WalletStatusUpdatePublisherMock { get; }

        public RpcFrontend Service { get; }

        public string CustomerId { get; }

        public RpcFrontendTestsFixture()
        {
            CustomerId = Guid.NewGuid().ToString();

            SessionCacheMock = new Mock<ISessionCache>(MockBehavior.Strict);
            BalanceUpdatePublisherMock = new Mock<IBalanceUpdatePublisher>(MockBehavior.Strict);
            WalletStatusUpdatePublisherMock = new Mock<IWalletStatusUpdatePublisher>();

            SessionCacheMock.Setup(x => x.GetClientId(It.IsAny<string>()))
                .Returns(CustomerId);

            BalanceUpdatePublisherMock
                .Setup(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<string>(),It.IsAny<Money18?>(), It.IsAny<DateTime?>()))
                .Returns(Task.CompletedTask);
            
            Service = new RpcFrontend(
                BalanceUpdatePublisherMock.Object,
                WalletStatusUpdatePublisherMock.Object,
                SessionCacheMock.Object);
        }
    }
}
