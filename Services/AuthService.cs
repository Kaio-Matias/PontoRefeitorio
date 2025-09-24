// Arquivo: PontoRefeitorio/Services/AuthService.cs

using System.Net.Http.Json;
using PontoRefeitorio.Helpers;
using PontoRefeitorio.Models;

namespace PontoRefeitorio.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://localhost:5114"; // <-- VERIFIQUE SE ESTE IP ESTÁ CORRETO

        public AuthService()
        {
            _httpClient = new HttpClient();
        }

        // Método que estava faltando:
        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var requestUrl = $"{_baseUrl}/api/auth/login";

            try
            {
                var response = await _httpClient.PostAsJsonAsync(requestUrl, loginRequest);

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
                // Trata erro de conexão
                return new LoginResponse { Message = $"Erro de conexão: {ex.Message}" };
            }

            return new LoginResponse { Message = "Usuário ou senha inválidos." };
        }
    }
}