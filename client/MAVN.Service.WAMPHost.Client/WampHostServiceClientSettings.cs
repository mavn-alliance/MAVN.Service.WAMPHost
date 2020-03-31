using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.WAMPHost.Client 
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
