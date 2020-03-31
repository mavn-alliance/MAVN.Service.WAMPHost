using System.Linq;
using MAVN.Service.WAMPHost.Domain.Services;
using WampSharp.V2.Authentication;
using WampSharp.V2.Core.Contracts;

namespace MAVN.Service.WAMPHost.DomainServices
{
    public class WampSessionAuthenticatorFactory : IWampSessionAuthenticatorFactory
    {
        private readonly ITokenValidator _tokenValidator;
        private readonly ISessionCache _sessionCache;
        private readonly string _realm;

        public WampSessionAuthenticatorFactory(
            ITokenValidator tokenValidator,
            ISessionCache sessionCache,
            string realm)
        {
            _tokenValidator = tokenValidator;
            _sessionCache = sessionCache;
            _realm = realm;
        }

        public IWampSessionAuthenticator GetSessionAuthenticator(
            WampPendingClientDetails details,
            IWampSessionAuthenticator transportAuthenticator)
        {
            if (details.Realm != _realm)
                throw new WampAuthenticationException(new AbortDetails { Message = "unknown realm" });

            if (details.HelloDetails.AuthenticationMethods != null && details.HelloDetails.AuthenticationMethods.Contains(AuthMethods.Ticket))
            {
                return new TicketSessionAuthenticator(details, _tokenValidator, _sessionCache);
            }

            return new AnonymousWampSessionAuthenticator();
        }
    }
}