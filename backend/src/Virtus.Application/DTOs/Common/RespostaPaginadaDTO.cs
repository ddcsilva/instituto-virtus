namespace Virtus.Application.DTOs.Common;

public class RespostaPaginadaDTO<T>
{
    public IEnumerable<T> Itens { get; set; } = [];
    public int TotalItens { get; set; }
    public int PaginaAtual { get; set; }
    public int ItensPorPagina { get; set; }
    public int TotalPaginas { get; set; }
    public bool TemPaginaAnterior => PaginaAtual > 1;
    public bool TemProximaPagina => PaginaAtual < TotalPaginas;
}
