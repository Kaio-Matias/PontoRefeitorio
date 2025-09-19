using Newtonsoft.Json;
using PontoRefeitorio.Helpers;
using System.Net.Http.Headers;
using System.Text;
using PontoRefeitorio.Models;

namespace PontoRefeitorio.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        // IMPORTANTE: Substitua pela URL da sua API
        private const string ApiBaseUrl = "https://10.1.0.51:8090/";

        public ApiService(AuthService authService)
        {
            _authService = authService;
            _httpClient = new HttpClient { BaseAddress = new Uri(ApiBaseUrl) };
        }

        public async Task<LoginResponse> LoginAsync(string email, string senha)
        {
            var deviceIdentifier = await DeviceInfoHelper.GetDeviceIdentifierAsync();
            var deviceName = DeviceInfoHelper.GetDeviceName();

            var request = new LoginRequest
            {
                Email = email,
                Senha = senha,
                DeviceIdentifier = deviceIdentifier,
                NomeDispositivo = deviceName
            };

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/auth/login", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // Lança uma exceção para ser tratada no ViewModel
                throw new Exception($"Erro ao fazer login: {responseContent}");
            }

            return JsonConvert.DeserializeObject<LoginResponse>(responseContent);
        }

        public async Task<RegistroPontoResponse> RegistrarPontoAsync(string colaboradorId, string photoPath)
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Usuário não autenticado.");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();

            // Adiciona a imagem
            var fileStream = File.OpenRead(photoPath);
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(streamContent, "ImageFile", Path.GetFileName(photoPath));

            // Adiciona o ID do Colaborador
            content.Add(new StringContent(colaboradorId), "ColaboradorId");

            var response = await _httpClient.PostAsync("/api/RegistroPonto/Registrar", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro ao registrar ponto: {responseContent}");
            }

            return JsonConvert.DeserializeObject<RegistroPontoResponse>(responseContent);
        }
    }
}
