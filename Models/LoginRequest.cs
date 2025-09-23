// Arquivo: PontoRefeitorio/Models/LoginRequest.cs

namespace PontoRefeitorio.Models
{
    public class LoginRequest
    {
        // Corrigido para Email, conforme seu modelo Usuario
        public string Email { get; set; }

        public string Senha { get; set; }
    }
}