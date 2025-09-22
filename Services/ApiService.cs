// PontoRefeitorio/Services/ApiService.cs

using PontoRefeitorio.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PontoRefeitorio.Services
{
    public class ApiService
    {
        public HttpClient HttpClient { get; }
        // Usando a URL correta da sua API publicada,
        private const string ApiBaseUrl = "http://10.1.0.51:8090/";

        public ApiService()
        {
            HttpClient = new HttpClient { BaseAddress = new Uri(ApiBaseUrl) };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("api/auth/login", content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }

        public async Task<RegistroPontoResponse> RegistrarPontoAsync(string token, string colaboradorId, string photoPath)
        {
            if (string.IsNullOrEmpty(token))
                throw new Exception("Usuário não autenticado.");

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(photoPath);
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            content.Add(streamContent, "ImageFile", Path.GetFileName(photoPath));
            content.Add(new StringContent(colaboradorId), "ColaboradorId");

            var response = await HttpClient.PostAsync("api/RegistroPonto/Registrar", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao registrar ponto: {responseContent}");

            // Usando o System.Text.Json para deserializar
            return JsonSerializer.Deserialize<RegistroPontoResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}