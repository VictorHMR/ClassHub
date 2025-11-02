using ClassHub.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClassHub.ClassHubContext.Models
{
    public class Usuario
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
        public TipoUsuario TipoUsuario { get; set; }

        public ICollection<Turma>? TurmasLecionadas { get; set; }
        public ICollection<AlunoTurma> Matriculas { get; set; } = new List<AlunoTurma>();


    }
}
