using Autofac;
using JetBrains.Annotations;
using Lykke.Common;
using Lykke.Sdk;
using Lykke.Service.PrivateBlockchainFacade.Client;
using Lykke.Service.Sessions.Client;
using Lykke.Service.WalletManagement.Client;
using Lykke.SettingsReader;
using MAVN.Service.WAMPHost.Domain.Services;
using MAVN.Service.WAMPHost.DomainServices;
using MAVN.Service.WAMPHost.DomainServices.Subscribers;
using MAVN.Service.WAMPHost.Managers;
using MAVN.Service.WAMPHost.Settings;
using WampSharp.V2;
using WampSharp.V2.Authentication;
using WampSharp.V2.Realm;

namespace MAVN.Service.WAMPHost.Modules
{
    [UsedImplicitly]
    public class ServiceModule : Module
    {
        private const string BalanceUpdatedExchangeName = "lykke.wallet.customerbalanceupdated";
        private const string CustomerWalletStatusUpdatedExchangeName = "lykke.wallet.walletstatusupdated";

        private readonly AppSettings _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings.CurrentValue;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();

            LoadClients(builder);

            LoadWamp(builder);

            LoadServices(builder);

            LoadRabbit(builder);
        }

        private void LoadRabbit(ContainerBuilder builder)
        {
            builder.RegisterType<BalanceUpdatedEventSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString",
                    _appSettings.WampHostService.RabbitMq.RabbitMqConnectionString)
                .WithParameter("exchangeName", BalanceUpdatedExchangeName);

            builder.RegisterType<CustomerWalletStatusUpdatedEventSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString",
                    _appSettings.WampHostService.RabbitMq.RabbitMqConnectionString)
                .WithParameter("exchangeName", CustomerWalletStatusUpdatedExchangeName);
        }

        private void LoadServices(ContainerBuilder builder)
        {
            builder.RegisterType<ClientResolver>()
                .As<ITokenValidator>()
                .As<ISessionCache>()
                .SingleInstance();

            builder.RegisterType<RpcFrontend>()
                .As<IRpcFrontend>()
                .SingleInstance();

            builder.RegisterType<BalanceUpdatePublisher>()
                .As<IBalanceUpdatePublisher>()
                .WithParameter("tokenSymbol",
                    _appSettings.Constants.TokenSymbol)
                .WithParameter("tokenFormatCultureInfo",
                    _appSettings.Constants.TokenFormatCultureInfo)
                .WithParameter("tokenNumberDecimalPlaces",
                    _appSettings.Constants.TokenNumberDecimalPlaces)
                .WithParameter("tokenIntegerPartFormat",
                    _appSettings.Constants.TokenIntegerPartFormat)
                .SingleInstance();

            builder.RegisterType<WalletStatusUpdatePublisher>()
                .As<IWalletStatusUpdatePublisher>()
                .SingleInstance();
        }

        private void LoadClients(ContainerBuilder builder)
        {
            builder.RegisterSessionsServiceClient(_appSettings.SessionServiceClient);

            builder.RegisterPrivateBlockchainFacadeClient(_appSettings.PrivateBlockchainFacadeService,
                null);

            builder.RegisterWalletManagementClient(_appSettings.WalletManagementServiceClient, null);
        }

        private void LoadWamp(ContainerBuilder builder)
        {
            builder.RegisterType<WampSessionAuthenticatorFactory>()
                .As<IWampSessionAuthenticatorFactory>()
                .WithParameter("realm", _appSettings.WampHostService.RealmName)
                .SingleInstance();

            builder.RegisterType<WampAuthenticationHost>()
                .As<IWampHost>()
                .SingleInstance();

            builder.RegisterType<WampRunner>()
                .As<IStartStop>()
                .SingleInstance();

            builder.Register(x =>
                x.Resolve<IWampHost>().RealmContainer
                    .GetRealmByName(_appSettings.WampHostService.RealmName));

            builder.Register(x =>
                {
                    var rpcMethods = x.Resolve<IRpcFrontend>();

                    var realm = x.Resolve<IWampHostedRealm>();

                    realm.Services.RegisterCallee(rpcMethods).GetAwaiter().GetResult();

                    return rpcMethods;
                })
                .AutoActivate();
        }
    }
}
