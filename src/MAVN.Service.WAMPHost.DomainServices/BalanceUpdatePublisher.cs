using System;
using System.Globalization;
using System.Threading.Tasks;
using Common.Log;
using MAVN.Numerics;
using Lykke.Common.Log;
using MAVN.Service.PrivateBlockchainFacade.Client;
using MAVN.Service.WAMPHost.Contract;
using MAVN.Service.WAMPHost.Domain.Services;
using WampSharp.V2;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Realm;

namespace MAVN.Service.WAMPHost.DomainServices
{
    public class BalanceUpdatePublisher : IBalanceUpdatePublisher
    {
        private readonly IPrivateBlockchainFacadeClient _privateBlockchainFacadeClient;
        private readonly IWampHostedRealm _realm;
        private readonly ISessionCache _sessionCache;
        private readonly string _tokenSymbol;
        private readonly string _tokenFormatCultureInfo;
        private readonly int _tokenNumberDecimalPlaces;
        private readonly string _tokenIntegerPartFormat;
        private readonly ILog _log;

        public BalanceUpdatePublisher(
            IPrivateBlockchainFacadeClient privateBlockchainFacadeClient,
            IWampHostedRealm realm,
            ISessionCache sessionCache,
            string tokenSymbol,
            string tokenFormatCultureInfo,
            int tokenNumberDecimalPlaces,
            string tokenIntegerPartFormat,
            ILogFactory logFactory)
        {
            _privateBlockchainFacadeClient = privateBlockchainFacadeClient;
            _realm = realm;
            _sessionCache = sessionCache;
            _tokenSymbol = tokenSymbol;
            _tokenFormatCultureInfo = tokenFormatCultureInfo;
            _tokenNumberDecimalPlaces = tokenNumberDecimalPlaces;
            _tokenIntegerPartFormat = tokenIntegerPartFormat;
            _log = logFactory.CreateLog(this);
        }

        public async Task PublishAsync(string customerId, string reason = default(string),
            Money18? balance = default(Money18?),
            DateTime? timestamp = default(DateTime?))
        {
            if (!balance.HasValue)
            {
                var balanceResponse =
                    await _privateBlockchainFacadeClient.CustomersApi.GetBalanceAsync(Guid.Parse(customerId));

                if (balanceResponse.Error != CustomerBalanceError.None)
                {
                    _log.Error(
                        null,
                        "Error fetching client balance.",
                        new {customerId},
                        process: nameof(PublishAsync));

                    return;
                }

                balance = balanceResponse.Total;
            }

            if (!timestamp.HasValue)
            {
                timestamp = DateTime.UtcNow;
            }

            var sessionIds = _sessionCache.GetSessionIds(customerId);

            if (sessionIds.Length == 0)
                return;

            var subject = _realm.Services.GetSubject(Topic.Balance);

            subject.OnNext(new WampEvent
            {
                Options = new PublishOptions {Eligible = sessionIds},
                Arguments = new object[]
                {
                    new BalanceUpdate
                    {
                        Balance = balance.Value.ToString(_tokenIntegerPartFormat, _tokenNumberDecimalPlaces,
                            new CultureInfo(_tokenFormatCultureInfo).NumberFormat),
                        Timestamp = timestamp.Value,
                        AssetSymbol = _tokenSymbol,
                        Reason = reason
                    }
                }
            });
        }
    }
}
