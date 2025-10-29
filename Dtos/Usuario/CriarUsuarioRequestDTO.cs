using ClassHub.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClassHub.Dtos.Usuario
{
    public class CriarUsuarioRequestDTO
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public string Senha { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
    }
}
