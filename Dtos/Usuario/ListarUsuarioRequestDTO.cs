using ClassHub.Enums;
using static ClassHub.Enums.Filtros;

namespace ClassHub.Dtos.Usuario
{
    public class ListarUsuarioRequestDTO
    {
        public int nrPagina { get; set; }
        public int qtRegistros { get; set; }
        public TipoUsuario? tipoUsuario { get; set; }
        public Ordenacao ordenacao { get; set; }
        public string pesquisa { get; set; }
        public int? idTurma { get; set; }
    }
}
