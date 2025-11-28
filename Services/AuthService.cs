using PontoRefeitorio.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PontoRefeitorio.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        // URL fixa da API
        public static string BaseUrl = "https://api.ponto-refeitorio.valedourado.com.br:8091";

        public AuthService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginRequest);
                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        await SecureStorage.SetAsync("auth_token", loginResponse.Token);
                        return loginResponse;
                    }
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse { Message = $"Erro de conexão: {ex.Message}" };
            }

            return new LoginResponse { Message = "Utilizador ou senha inválidos." };
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync("/api/auth/validate-token");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
