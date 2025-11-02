using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassHub.Dtos.Turma
{
    public class CriarTurmaRequestDTO
    {
        public string Nome { get; set; }
        public int IdProfessor { get; set; }
    }
}
