using System.Threading.Tasks;

namespace PontoRefeitorio.Services
{
    public class AuthService
    {
        private const string AuthTokenKey = "auth_token";

        public async Task<string> GetTokenAsync()
        {
            return await SecureStorage.GetAsync(AuthTokenKey);
        }

        public async Task SetTokenAsync(string token)
        {
            await SecureStorage.SetAsync(AuthTokenKey, token);
        }

        public void Logout()
        {
            SecureStorage.Remove(AuthTokenKey);
        }
    }
}
