using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.WAMPHost.Settings
{
    [UsedImplicitly]
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string RabbitMqConnectionString { get; set; }
    }
}