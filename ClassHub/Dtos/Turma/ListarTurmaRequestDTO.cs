using ClassHub.Enums;

namespace ClassHub.Dtos.Turma
{
    public class ListarTurmaRequestDTO
    {
        public int? idUsuario { get; set; } = null;
        public int nrPagina { get; set; } = 1;
        public int qtRegistros { get; set; } = 10;
        public string pesquisa { get; set; } = "";
        public Filtros.Ordenacao ordenacao { get; set; } = Filtros.Ordenacao.Ascendente;
    }
}
