using Newtonsoft.Json;
using PontoRefeitorio.Models;
using System.Net.Http.Headers;

namespace PontoRefeitorio.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5114"; // IP para emulador Android

        public ApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<RegistroPontoResponse> RegistrarPonto(byte[] fotoBytes, string fileName = "ponto.jpg")
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(fotoBytes), "file", fileName);

                var response = await _httpClient.PostAsync("/api/Identificacao/registrar-ponto", content);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<RegistroPontoResponse>(jsonResponse);
            }
            catch (Exception ex)
            {
                return new RegistroPontoResponse { Sucesso = false, Mensagem = "Não foi possível conectar ao servidor." };
            }
        }
    }
}