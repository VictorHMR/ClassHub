namespace ClassHub.Dtos.Turma
{
    public class EditarTurmaRequestDTO
    {
        public int IdTurma { get; set; }
        public string Nome { get; set; }
        public int IdProfessor { get; set; }
        public DateTime? DtFim { get; set; }
    }
}
