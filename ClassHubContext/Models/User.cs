using ClassHub.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClassHub.ClassHubContext.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string RA { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string CPF { get; set; }
        [Required]
        public string Senha { get; set; }
        [Required]
        public Role Role { get; set; }
    }
}
