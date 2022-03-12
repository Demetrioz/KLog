namespace KLog.Api.Services
{
    public interface ISecurityService
    {
        (string, string) GenerateKey();
        string GenerateKeyHash(string key);
        bool ValidateKey(string hashedKey, string plainKey);
    }
}
