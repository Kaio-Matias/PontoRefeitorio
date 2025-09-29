using Newtonsoft.Json;
using PontoRefeitorio.Models;
using System.Net.Http.Headers;

namespace PontoRefeitorio.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        // --- INÍCIO DA CORREÇÃO ---
        // 1. IP corrigido para comunicação com o emulador Android.
        // Verifique se a porta 5114 é a mesma que sua API está usando.
        private const string BaseUrl = "http://localhost:5114";
        // --- FIM DA CORREÇÃO ---

        public ApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<RegistroPontoResponse> RegistrarPonto(byte[] fotoBytes, string fileName = "ponto.jpg")
        {
            try
            {
                // Busca o token de autenticação salvo no dispositivo
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token))
                {
                    return new RegistroPontoResponse { Sucesso = false, Mensagem = "Usuário não autenticado. Faça o login novamente." };
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using var content = new MultipartFormDataContent();
                // O nome "file" deve corresponder ao parâmetro no controller ([FromForm] IFormFile file)
                content.Add(new ByteArrayContent(fotoBytes), "file", fileName);

                // Endpoint corrigido para corresponder ao IdentificacaoController
                var response = await _httpClient.PostAsync("/api/Identificacao/registrar-ponto", content);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<RegistroPontoResponse>(jsonResponse);
                }
                else
                {
                    // Tenta desserializar a resposta de erro para obter uma mensagem mais clara
                    var errorResponse = JsonConvert.DeserializeObject<RegistroPontoResponse>(jsonResponse);
                    return errorResponse ?? new RegistroPontoResponse { Sucesso = false, Mensagem = $"Erro {response.StatusCode}: {jsonResponse}" };
                }
            }
            catch (Exception ex)
            {
                // Log do erro pode ser adicionado aqui
                return new RegistroPontoResponse { Sucesso = false, Mensagem = "Não foi possível conectar ao servidor. Verifique sua conexão." };
            }
        }
    }
}