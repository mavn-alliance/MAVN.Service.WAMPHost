using Lykke.HttpClientGenerator;

namespace Lykke.Service.WampHost.Client
{
    /// <summary>
    /// WampHost API aggregating interface.
    /// </summary>
    public class WampHostClient : IWampHostClient
    {
        // Note: Add similar Api properties for each new service controller

        /// <summary>Inerface to WampHost Api.</summary>
        public IWampHostApi Api { get; private set; }

        /// <summary>C-tor</summary>
        public WampHostClient(IHttpClientGenerator httpClientGenerator)
        {
            Api = httpClientGenerator.Generate<IWampHostApi>();
        }
    }
}
