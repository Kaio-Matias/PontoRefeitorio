using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PontoRefeitorio.Models
{
    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public string? DeviceIdentifier { get; set; }
        public string? NomeDispositivo { get; set; }
    }
}
