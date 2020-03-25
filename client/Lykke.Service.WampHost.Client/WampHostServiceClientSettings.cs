using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.WampHost.Client 
{
    /// <summary>
    /// WampHost client settings.
    /// </summary>
    public class WampHostServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
