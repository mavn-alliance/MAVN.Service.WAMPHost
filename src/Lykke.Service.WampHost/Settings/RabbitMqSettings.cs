using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.WampHost.Settings
{
    [UsedImplicitly]
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string RabbitMqConnectionString { get; set; }
    }
}