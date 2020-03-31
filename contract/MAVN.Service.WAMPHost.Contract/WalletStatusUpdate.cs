namespace MAVN.Service.WAMPHost.Contract
{
    /// <summary>
    /// Represents the wallet status.
    /// </summary>
    public class WalletStatusUpdate
    {
        /// <summary>
        /// Indicates that the wallet is blocked.
        /// </summary>
        public bool IsBlocked { get; set; }
    }
}
