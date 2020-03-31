using JetBrains.Annotations;

namespace MAVN.Service.WAMPHost.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class WampHostSettings
    {
        public DbSettings Db { get; set; }
        
        public RabbitMqSettings RabbitMq { get; set; }
        
        public string RealmName { get; set; }
    }
}
