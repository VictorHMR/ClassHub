namespace ClassHub.Dtos.Turma
{
    public class TurmaDTO
    {
        public int IdTurma { get; set; }
        public string Nome { get; set; }
        public DateTime DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        public int QtdAlunos { get; set; }
        public int IdProfessor { get; set; }
        public string NomeProfessor { get; set; }
    }
}
