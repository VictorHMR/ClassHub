namespace ClassHub.Dtos.Nota
{
    public class ListarNotasAlunoResponseDTO
    {
        public int IdNota { get; set; }
        public double Nota { get; set; }
        public string Descricao { get; set; }
        public DateTime DtLancamento { get; set; }
    }
}
