using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.WampHost.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
