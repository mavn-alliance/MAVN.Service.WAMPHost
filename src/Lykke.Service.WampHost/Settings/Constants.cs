using JetBrains.Annotations;

namespace Lykke.Service.WampHost.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class Constants
    {
        public string TokenSymbol { get; set; }

        public string TokenFormatCultureInfo { get; set; }

        public int TokenNumberDecimalPlaces { get; set; }

        public string TokenIntegerPartFormat { get; set; }
    }
}
