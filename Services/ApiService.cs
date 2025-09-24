// Arquivo: PontoRefeitorio/Services/ApiService.cs
using System.Net.Http.Headers;
using System.Text.Json;
using PontoRefeitorio.Helpers;
using PontoRefeitorio.Models;

namespace PontoRefeitorio.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://localhost:5114";

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
            _httpClient.DefaultRequestHeaders.Remove("DeviceId");
            _httpClient.DefaultRequestHeaders.Add("DeviceId", DeviceInfoHelper.GetDeviceId());

            using var content = new MultipartFormDataContent();
            var imageContent = new ByteArrayContent(photoBytes);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(imageContent, "fotoFile", "photo.jpg"); // O nome "fotoFile" deve corresponder ao parâmetro na API

            // ==================================================================
            // INÍCIO DA CORREÇÃO
            // ==================================================================
            // Corrigido para o controlador e método corretos da sua API
            var requestUrl = $"{_baseUrl}/api/Identificacao/identificar";
            // ==================================================================
            // FIM DA CORREÇÃO
            // ==================================================================

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
                    // Tenta deserializar a mensagem de erro da API
                    var errorResponse = JsonSerializer.Deserialize<RegistroPontoResponse>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return new RegistroPontoResponse { Sucesso = false, Mensagem = errorResponse?.Mensagem ?? $"Falha na API: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                return new RegistroPontoResponse { Sucesso = false, Mensagem = $"Erro de conexão: {ex.Message}" };
            }
        }
    }
}