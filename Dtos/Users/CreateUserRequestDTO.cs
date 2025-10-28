using ClassHub.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClassHub.Dtos.Users
{
    public class CreateUserRequestDTO
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public string Senha { get; set; }
        public Role Role { get; set; }
    }
}
