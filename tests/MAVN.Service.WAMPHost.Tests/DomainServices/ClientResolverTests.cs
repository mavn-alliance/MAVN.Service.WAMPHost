using System;
using Xunit;

namespace MAVN.Service.WAMPHost.Tests.DomainServices
{
    public class ClientResolverTests
    {
        [Fact]
        public void ShouldValidate_True_WhenExistingTokenPassed()
        {
            // Arrange
            var fixture = new ClientResolverTestsFixture();
            
            // Act
            var isValid = fixture.Service.Validate(fixture.Token);
            
            // Assert
            Assert.True(isValid);
        }
        
        [Fact]
        public void ShouldValidate_False_WhenExistingTokenPassed()
        {
            // Arrange
            var fixture = new ClientResolverTestsFixture();
            
            // Act
            var isValid = fixture.Service.Validate(Guid.NewGuid().ToString());
            
            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Should_GetCustomerId_When_ValidTokenPassed()
        {
            // Arrange
            var fixture = new ClientResolverTestsFixture();
            
            // Act
            var clientId = fixture.Service.GetClientId(fixture.Token);
            
            // Assert
            Assert.Equal(fixture.CustomerId, clientId);
        }

        [Fact]
        public void Should_Not_GetCustomerId_When_Not_ValidTokenPassed()
        {
            // Arrange
            var fixture = new ClientResolverTestsFixture();
            
            // Act
            var clientId = fixture.Service.GetClientId(Guid.NewGuid().ToString());
            
            // Assert
            Assert.Null(clientId);
        }

        [Fact]
        public void Saves_Tokens()
        {
            // Arrange
            var fixture = new ClientResolverTestsFixture();

            var session1 = 7654532465456234645;
            var session2 = 6294580234578243589;
            
            // Act
            fixture.Service.AddSessionId(fixture.Token, session1);
            fixture.Service.AddSessionId(fixture.Token, session2);

            var result = fixture.Service.GetSessionIds(fixture.CustomerId);
            
            // Assert
            Assert.Equal(2, result.Length);
            Assert.Contains(session1, result);
            Assert.Contains(session2, result);
        }
    }
}