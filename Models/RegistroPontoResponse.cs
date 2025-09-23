// Arquivo: PontoRefeitorio/Models/RegistroPontoResponse.cs

namespace PontoRefeitorio.Models
{
    public class RegistroPontoResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }

        // Propriedades que estavam faltando:
        public string Nome { get; set; }
        public string FotoBase64 { get; set; }
    }
}