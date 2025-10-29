using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassHub.ClassHubContext.Models
{
    public class AlunoTurma
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Aluno))]
        public int IdAluno { get; set; }

        [Required]
        [ForeignKey(nameof(Turma))]
        public int IdTurma { get; set; }

        public Usuario Aluno { get; set; } = null!;
        public Turma Turma { get; set; } = null!;

        public DateTime DtMatricula { get; set; } = DateTime.Now;
    }
}
