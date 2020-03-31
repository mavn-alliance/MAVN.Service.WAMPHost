namespace MAVN.Service.WAMPHost.Domain.Services
{
    public interface ITokenValidator
    {
        bool Validate(string token);
    }
}