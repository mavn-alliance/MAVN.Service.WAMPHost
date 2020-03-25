using JetBrains.Annotations;
using Lykke.Common;
using WampSharp.V2;

namespace Lykke.Service.WampHost.DomainServices
{
    [UsedImplicitly]
    public class WampRunner : IStartStop
    {
        private readonly IWampHost _wampHost;

        public WampRunner(IWampHost wampHost)
        {
            _wampHost = wampHost;
        }

        public void Start()
        {
            _wampHost.Open();
        }

        public void Dispose()
        {
            _wampHost.Dispose();
        }

        public void Stop()
        {
            _wampHost.Dispose();
        }
    }
}
