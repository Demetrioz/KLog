using KLog.DataModel.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace KLog.Api.Services
{
    public interface IAuthenticationService
    {
        (string, string) GenerateApiKey();
        string GeneratePublicKey();
        string GenerateJwt(User user);
        JwtSecurityToken DecodeJwt(string jwt);

        string GenerateHash(string key);
        bool ValidateHash(string hashedKey, string plainKey);

        string Decrypt(string key);
    }
}
