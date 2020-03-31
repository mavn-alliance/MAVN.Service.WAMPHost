using JetBrains.Annotations;

namespace MAVN.Service.WAMPHost.Client
{
    /// <summary>
    /// WampHost client interface.
    /// </summary>
    [PublicAPI]
    public interface IWampHostClient
    {
        // Make your app's controller interfaces visible by adding corresponding properties here.
        // NO actual methods should be placed here (these go to controller interfaces, for example - IWampHostApi).
        // ONLY properties for accessing controller interfaces are allowed.

        /// <summary>Application Api interface</summary>
        IWampHostApi Api { get; }
    }
}
