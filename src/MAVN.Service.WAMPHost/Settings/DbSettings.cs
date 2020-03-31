using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.WAMPHost.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
