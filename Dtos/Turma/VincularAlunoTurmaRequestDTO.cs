namespace ClassHub.Dtos.Turma
{
    public class VincularAlunoTurmaRequestDTO
    {
        public string RAAluno { get; set; }
        public int IdTurma { get; set; }
        public bool FlDesvincular { get; set; } = false;
    }
}
