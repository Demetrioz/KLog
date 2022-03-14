using KLog.DataModel.Entities;

namespace KLog.Api.Services
{
    public interface IAuthenticationService
    {
        (string, string) GenerateApiKey();
        string GeneratePublicKey();
        string GenerateJwt(User user);

        string GenerateHash(string key);
        bool ValidateHash(string hashedKey, string plainKey);

        string Decrypt(string key);
    }
}
