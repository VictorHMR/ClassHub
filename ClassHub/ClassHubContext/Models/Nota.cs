using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassHub.ClassHubContext.Models
{
    public class Nota
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey(nameof(AlunoTurma))]
        public int IdAlunoTurma { get; set; }
        [Required]
        public double Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime DtLancamento { get; set; } = DateTime.Now;

        public AlunoTurma AlunoTurma { get; set; }
    }
}
