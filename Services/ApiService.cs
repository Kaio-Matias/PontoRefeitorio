// Arquivo: PontoRefeitorio/Services/ApiService.cs
using System.Net.Http;
using System.Threading.Tasks;
using PontoRefeitorio.Models;
using System;
using System.IO;
using Newtonsoft.Json; // Adicione o pacote NuGet Newtonsoft.Json ao seu projeto

namespace PontoRefeitorio.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        // ATENÇÃO: Use o IP da sua máquina na rede local, não localhost.
        // O emulador Android acessa o localhost da máquina host através do IP 10.0.2.2
        // Se estiver testando em um dispositivo físico, ambos devem estar na mesma rede.
        private readonly string _baseUrl = "http://localhost:5114";

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // MÉTODO TOTALMENTE CORRIGIDO
        public async Task<RegistroPontoResponse> RegistrarPonto(byte[] fotoBytes, string fileName = "ponto.jpg")
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                using (var content = new MultipartFormDataContent())
                {
                    // O nome "file" deve corresponder ao parâmetro [FromForm] IFormFile file na sua API
                    content.Add(new ByteArrayContent(fotoBytes), "file", fileName);

                    // O endpoint agora aponta para o novo IdentificacaoController
                    var response = await _httpClient.PostAsync($"{_baseUrl}/api/Identificacao/registrar-ponto", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        // Deserializa a resposta da API para o objeto de modelo
                        return JsonConvert.DeserializeObject<RegistroPontoResponse>(jsonResponse);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Erro na API: {response.StatusCode} - {errorContent}");
                        // Tenta deserializar uma mensagem de erro, se houver
                        var errorResponse = JsonConvert.DeserializeObject<RegistroPontoResponse>(errorContent);
                        return new RegistroPontoResponse { Sucesso = false, Mensagem = errorResponse?.Mensagem ?? "Servidor retornou um erro." };
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Erro de conexão com a API: {httpEx.Message}");
                return new RegistroPontoResponse { Sucesso = false, Mensagem = "Não foi possível conectar ao servidor." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado ao registrar ponto: {ex.Message}");
                return new RegistroPontoResponse { Sucesso = false, Mensagem = "Ocorreu um erro no aplicativo." };
            }
        }

        // O método antigo foi removido para evitar confusão.
    }
}