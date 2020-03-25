namespace Lykke.Service.WampHost.DomainServices
{
    static class Topic
    {
        public const string Balance = "balance";
        public const string WalletStatus = "wallet-status";
    }

    static class Topics
    {
        public static readonly string[] WithAuth = {Topic.Balance, Topic.WalletStatus};
    }
}
