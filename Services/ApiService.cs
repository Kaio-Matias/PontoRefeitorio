using Newtonsoft.Json;
using PontoRefeitorio.Models;
using System.Net.Http.Headers;

namespace PontoRefeitorio.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        // Garante que a URL base seja consistente em toda a aplicação.
        public static string BaseUrl => AuthService.BaseUrl;

        public ApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<RegistroPontoResponse> RegistrarPonto(byte[] fotoBytes, CancellationToken cancellationToken, string fileName = "ponto.jpg")
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

                // Passa o CancellationToken para a requisição, permitindo o cancelamento.
                var response = await _httpClient.PostAsync("api/Identificacao/registrar-ponto", content, cancellationToken);

                // Sempre lê o conteúdo da resposta, seja sucesso ou erro.
                var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);

                // Se a requisição não teve sucesso (ex: 400, 404, 500)
                if (!response.IsSuccessStatusCode)
                {
                    // Tenta extrair a mensagem de erro específica da API.
                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        var errorResponse = JsonConvert.DeserializeObject<RegistroPontoResponse>(jsonResponse);
                        if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Mensagem))
                        {
                            return new RegistroPontoResponse { Sucesso = false, Mensagem = errorResponse.Mensagem };
                        }
                    }

                    // Fallback para mensagens de erro genéricas.
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return new RegistroPontoResponse { Sucesso = false, Mensagem = "Sessão expirada. Faça o login novamente." };
                    }

                    return new RegistroPontoResponse { Sucesso = false, Mensagem = $"Erro do servidor: {response.ReasonPhrase}" };
                }

                // Se a requisição teve sucesso (código 2xx).
                return JsonConvert.DeserializeObject<RegistroPontoResponse>(jsonResponse);
            }
            catch (OperationCanceledException)
            {
                // Propaga a exceção para que o ViewModel saiba que a operação foi cancelada.
                throw;
            }
            catch (Exception)
            {
                // Ocorre se a API estiver offline ou inacessível.
                return new RegistroPontoResponse { Sucesso = false, Mensagem = "Não foi possível conectar ao servidor." };
            }
        }
    }
}