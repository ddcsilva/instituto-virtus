using Virtus.Application.DTOs.Pagamento;
using Virtus.Application.DTOs.Common;

namespace Virtus.Application.Interfaces;

public interface IServicoPagamento
{
    public Task<ResultadoDTO<PagamentoDTO>> ObterPorIdAsync(int id);
    public Task<ResultadoDTO<PagamentoDTO>> CriarAsync(CriarPagamentoDTO dto);
    public Task<RespostaPaginadaDTO<PagamentoDTO>> ObterPorPeriodoAsync(
        DateTime dataInicial,
        DateTime dataFinal,
        int pagina = 1,
        int itensPorPagina = 20);
    public Task<IEnumerable<PagamentoDTO>> ObterPorAlunoAsync(int alunoId);
    public Task<IEnumerable<PagamentoDTO>> ObterPorPagadorAsync(int pagadorId);
}
