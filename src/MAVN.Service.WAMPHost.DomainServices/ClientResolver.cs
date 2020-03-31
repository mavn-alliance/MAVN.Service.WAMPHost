using System;
using System.Collections.Generic;
using System.Linq;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.Sessions.Client;
using MAVN.Service.WAMPHost.Domain.Services;
using Microsoft.Extensions.Caching.Memory;

namespace MAVN.Service.WAMPHost.DomainServices
{
    public class ClientResolver : ITokenValidator, ISessionCache
    {
        private static readonly long[] ZeroSessionsValue = new long[0];

        private readonly ILog _log;
        private readonly IMemoryCache _cache;
        [NotNull]
        private readonly ISessionsServiceClient _sessionsServiceClient;

        private readonly MemoryCacheEntryOptions _tokenCacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
        private readonly MemoryCacheEntryOptions _sessionCacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(14));

        public ClientResolver(
            [NotNull] ILogFactory logFactory,
            [NotNull] ISessionsServiceClient sessionsServiceClient,
            [NotNull] IMemoryCache cache)
        {
            _log = logFactory.CreateLog(this) ?? throw new ArgumentNullException(nameof(logFactory));
            _sessionsServiceClient = sessionsServiceClient ?? throw new ArgumentNullException(nameof(sessionsServiceClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public bool Validate(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            return GetClientId(token) != null;
        }

        public string GetClientId(string token)
        {
            if (_cache.TryGetValue(token, out string clientId))
                return clientId;

            try
            {
                clientId = _sessionsServiceClient.SessionsApi.GetSessionAsync(token).GetAwaiter().GetResult()?.ClientId;
            }
            catch (Exception exception)
            {
                _log.Error(exception);
                return null;
            }

            if (clientId != null)
                _cache.Set(token, clientId, _tokenCacheOptions);

            return clientId;
        }

        public long[] GetSessionIds(string clientId)
        {
            if (_cache.TryGetValue(clientId, out long[] sessionIds))
                return sessionIds;

            return ZeroSessionsValue;
        }

        public void AddSessionId(string token, long sessionId)
        {
            var clientId = GetClientId(token);
            _cache.Set(sessionId, clientId, _sessionCacheOptions);

            if (_cache.TryGetValue(clientId, out long[] sessionIds))
            {
                // working with HashSet is not effective, but more readable than working with Array; type 'long[]' is stored for performance needs
                var sessions = new HashSet<long>(sessionIds) { sessionId };
                _cache.Set(clientId, sessions.ToArray(), _sessionCacheOptions);
            }
            else
            {
                _cache.Set(clientId, new[] { sessionId }, _sessionCacheOptions);
            }
        }
    }
}
