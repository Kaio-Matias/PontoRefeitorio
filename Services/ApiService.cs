using Newtonsoft.Json;
using PontoRefeitorio.Models;
using System.Net.Http.Headers;
using System.Text; // Adicionar para o Json

namespace PontoRefeitorio.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        public static string BaseUrl => AuthService.BaseUrl;

        public ApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<RegistroPontoResponse> RegistrarPonto(byte[] fotoBytes, string fileName = "ponto.jpg")
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token))
                {
                    return new RegistroPontoResponse { Sucesso = false, Mensagem = "Não autorizado. Faça o login novamente." };
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(fotoBytes), "file", fileName);

                var response = await _httpClient.PostAsync("api/Identificacao/registrar-ponto", content);

                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Se a resposta NÃO for de sucesso (ex: 400 Bad Request, 404 Not Found)
                if (!response.IsSuccessStatusCode)
                {
                    // Tenta ler a mensagem de erro específica enviada pela API
                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        var errorResponse = JsonConvert.DeserializeObject<RegistroPontoResponse>(jsonResponse);
                        if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Mensagem))
                        {
                            return new RegistroPontoResponse { Sucesso = false, Mensagem = errorResponse.Mensagem };
                        }
                    }

                    // Se não conseguir ler a mensagem específica, retorna um erro genérico
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return new RegistroPontoResponse { Sucesso = false, Mensagem = "Sessão expirada. Faça o login novamente." };
                    }

                    return new RegistroPontoResponse { Sucesso = false, Mensagem = $"Erro do servidor: {response.ReasonPhrase}" };
                }

                // Se a resposta for de sucesso (200 OK)
                return JsonConvert.DeserializeObject<RegistroPontoResponse>(jsonResponse);
            }
            catch (Exception ex)
            {
                // Este erro geralmente indica que a API está offline ou o endereço/porta está errado.
                System.Diagnostics.Debug.WriteLine($"Erro de conexão: {ex.Message}");
                return new RegistroPontoResponse { Sucesso = false, Mensagem = "Não foi possível conectar ao servidor." };
            }
        }
    }
}