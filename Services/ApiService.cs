using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PontoRefeitorio.Models;
using System;
using System.IO;

namespace PontoRefeitorio.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://10.1.0.51:8090"; // ou o seu URL de produção/teste

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> RegistrarPonto()
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/Identificacao/registrar-ponto", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // NOVO MÉTODO ADICIONADO
        public async Task<bool> EnviarFotoParaReconhecimento(Stream fotoStream, string fileName)
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                using (var content = new MultipartFormDataContent())
                {
                    // O nome "file" deve corresponder ao parâmetro [FromForm] IFormFile file na sua API
                    content.Add(new StreamContent(fotoStream), "file", fileName);

                    var response = await _httpClient.PostAsync($"{_baseUrl}/api/Reconhecimento/detectar-rosto", content);

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar foto para reconhecimento: {ex.Message}");
                return false;
            }
        }
    }
}