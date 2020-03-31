using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.PrivateBlockchainFacade.Client;
using Lykke.Service.Sessions.Client;
using Lykke.Service.WalletManagement.Client;

namespace MAVN.Service.WAMPHost.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public WampHostSettings WampHostService { get; set; }

        public Constants Constants { get; set; }

        public SessionsServiceClientSettings SessionServiceClient { get; set; }

        public PrivateBlockchainFacadeServiceClientSettings PrivateBlockchainFacadeService { get; set; }

        public WalletManagementServiceClientSettings WalletManagementServiceClient { get; set; }
    }
}
