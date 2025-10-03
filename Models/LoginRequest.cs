// Arquivo: PontoRefeitorio/Models/LoginRequest.cs
namespace PontoRefeitorio.Models
{
    public class LoginRequest
    {
        // CORREÇÃO: Alterado de Email para Username.
        public string Username { get; set; }
        // CORREÇÃO: Alterado de Senha para Password.
        public string Password { get; set; }
    }
}