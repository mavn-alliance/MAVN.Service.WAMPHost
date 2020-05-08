using System;
using Lykke.Logs;
using MAVN.Service.Sessions.Client;
using MAVN.Service.Sessions.Client.Models;
using MAVN.Service.WAMPHost.DomainServices;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace MAVN.Service.WAMPHost.Tests.DomainServices
{
    public class ClientResolverTestsFixture
    {
        public ClientResolver Service { get; }
        
        private Mock<ISessionsServiceClient> SessionsServiceClientMock;
        
        public string CustomerId { get; }
        public string Token { get; }
        
        public long[] SessionIds { get; }
        
        public ClientResolverTestsFixture()
        {
            SessionsServiceClientMock = new Mock<ISessionsServiceClient>();

            Token = Guid.NewGuid().ToString();
            CustomerId = Guid.NewGuid().ToString();

            SessionsServiceClientMock.Setup(c => c.SessionsApi.GetSessionAsync(It.IsAny<string>()))
                .ReturnsAsync((string c) =>
                {
                    if (c == Token)
                        return new ClientSession
                        {
                            ClientId = CustomerId,
                            SessionToken = c
                        };
                    else
                        return null;
                });

            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            memoryCache.Set(CustomerId, SessionIds);
            
            Service = new ClientResolver(
                EmptyLogFactory.Instance,
                SessionsServiceClientMock.Object,
                memoryCache);
        }
    }
}
