using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using PontoRefeitorio.Helpers;
using PontoRefeitorio.Models;

namespace PontoRefeitorio.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://10.1.0.51:8090"; // <-- VERIFIQUE SE ESTE IP ESTÁ CORRETO

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<RegistroPontoResponse> RegistrarPonto(byte[] photoBytes)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
            {
                return new RegistroPontoResponse { Sucesso = false, Mensagem = "Dispositivo não autenticado." };
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Remove("DeviceId"); // Limpa o header antes de adicionar
            _httpClient.DefaultRequestHeaders.Add("DeviceId", DeviceInfoHelper.GetDeviceId());

            // Usamos MultipartFormDataContent para enviar a imagem
            using var content = new MultipartFormDataContent();
            var imageContent = new ByteArrayContent(photoBytes);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            // O nome "file" deve ser o mesmo esperado pelo seu endpoint na API
            content.Add(imageContent, "file", "photo.jpg");

            var requestUrl = $"{_baseUrl}/api/reconhecimento/rosto";

            try
            {
                var response = await _httpClient.PostAsync(requestUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<RegistroPontoResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new RegistroPontoResponse { Sucesso = false, Mensagem = $"Falha na API: {response.StatusCode} - {errorContent}" };
                }
            }
            catch (Exception ex)
            {
                return new RegistroPontoResponse { Sucesso = false, Mensagem = $"Erro de conexão: {ex.Message}" };
            }
        }
    }
}