using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassHub.ClassHubContext.Models
{
    public class Turma
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public DateTime DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        [Required]
        [ForeignKey(nameof(Professor))]
        public int IdProfessor { get; set; }

        public Usuario Professor { get; set; } = null!;

        public ICollection<AlunoTurma> Matriculas { get; set; } = new List<AlunoTurma>();


    }
}
