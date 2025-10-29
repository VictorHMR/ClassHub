namespace ClassHub.ClassHubContext.Models
{
    public class PaginacaoResult<T>
    {
        public IEnumerable<T> Itens { get; set; } = new List<T>();
        public int PaginaAtual { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalItens { get; set; }
        public int TotalPaginas { get; set; }
    }
}
