using System;

namespace MAVN.Service.WAMPHost.Contract
{
    /// <summary>
    /// Balance update
    /// </summary>
    public class BalanceUpdate
    {
        /// <summary>Balance</summary>
        public string Balance { get; set; }

        /// <summary>Timestamp</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Asset symbol</summary>
        public string AssetSymbol { get; set; }

        /// <summary>Reason</summary>
        public string Reason { get; set; }
    }
}
