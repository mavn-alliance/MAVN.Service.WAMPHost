using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.WampHost.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class WampHostSettings
    {
        public DbSettings Db { get; set; }
        
        public RabbitMqSettings RabbitMq { get; set; }
        
        public string RealmName { get; set; }
    }
}
