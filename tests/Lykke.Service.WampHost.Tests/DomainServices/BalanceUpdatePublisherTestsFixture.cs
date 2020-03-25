using System;
using Lykke.Logs;
using Lykke.Service.PrivateBlockchainFacade.Client;
using Lykke.Service.PrivateBlockchainFacade.Client.Models;
using Lykke.Service.WampHost.Domain.Services;
using Lykke.Service.WampHost.DomainServices;
using Moq;
using WampSharp.V2;
using WampSharp.V2.Realm;

namespace Lykke.Service.WampHost.Tests.DomainServices
{
    public class BalanceUpdatePublisherTestsFixture
    {
        public BalanceUpdatePublisher Service { get; }
        
        public Mock<IPrivateBlockchainFacadeClient> BlockchainFacadeClientMock { get; }
        public Mock<IWampHostedRealm> WampHostedRealmMock { get; }
        public Mock<IWampSubject> WampSubjectMock { get; }
        public Mock<ISessionCache> SessionCacheMock { get; }
        
        public string CustomerId { get; }
        public long BalanceFromService { get; }
        public long SessionId { get; }

        public BalanceUpdatePublisherTestsFixture()
        {
            CustomerId = Guid.NewGuid().ToString();
            BalanceFromService = 100;
            SessionId = 1134123414351234;
            
            BlockchainFacadeClientMock = new Mock<IPrivateBlockchainFacadeClient>();
            WampSubjectMock = new Mock<IWampSubject>();
            WampHostedRealmMock = new Mock<IWampHostedRealm>();
            SessionCacheMock = new Mock<ISessionCache>();

            SessionCacheMock.Setup(x => x.GetSessionIds(It.IsAny<string>()))
                .Returns(new [] {SessionId});

            WampSubjectMock.Setup(x => x.OnNext(It.IsAny<IWampEvent>()));

            WampHostedRealmMock.Setup(x => x.Services.GetSubject(It.IsAny<string>()))
                .Returns(WampSubjectMock.Object);

            BlockchainFacadeClientMock.Setup(x => x.CustomersApi.GetBalanceAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new CustomerBalanceResponseModel
                {
                    Total = BalanceFromService
                });
            
            Service = new BalanceUpdatePublisher(
                BlockchainFacadeClientMock.Object,
                WampHostedRealmMock.Object,
                SessionCacheMock.Object,
                "MVN",
                "en-US",
                2,
                "N0",
                EmptyLogFactory.Instance);
        }
    }
}
