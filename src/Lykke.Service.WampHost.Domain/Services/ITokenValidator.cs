namespace Lykke.Service.WampHost.Domain.Services
{
    public interface ITokenValidator
    {
        bool Validate(string token);
    }
}