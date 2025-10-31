using ClassHub.Enums;

namespace ClassHub.Dtos.Usuario
{
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }
        public string RA { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public TipoUsuario TipoUsuario { get; set; }


    }
}
