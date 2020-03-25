namespace Lykke.Service.WampHost.Domain.Services
{
    public interface ISessionCache
    {
        long[] GetSessionIds(string clientId);
        void AddSessionId(string token, long sessionId);
        string GetClientId(string token);
    }
}