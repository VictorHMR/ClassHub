using ClassHub.Enums;

namespace ClassHub.Dtos.Usuario
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
    }
}
