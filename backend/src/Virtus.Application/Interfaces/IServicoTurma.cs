using Virtus.Application.DTOs.Common;
using Virtus.Application.DTOs.Turma;
using Virtus.Domain.Enums;

namespace Virtus.Application.Interfaces;

public interface IServicoTurma
{
    public Task<ResultadoDTO<TurmaDTO>> ObterPorIdAsync(int id);
    public Task<RespostaPaginadaDTO<TurmaListaDTO>> ObterTodasAsync(int pagina = 1, int itensPorPagina = 20);
    public Task<ResultadoDTO<TurmaDTO>> CriarAsync(CriarTurmaDTO dto);
    public Task<ResultadoDTO<TurmaDTO>> AtualizarAsync(int id, AtualizarTurmaDTO dto);
    public Task<ResultadoDTO<bool>> InativarAsync(int id);
    public Task<ResultadoDTO<bool>> AtivarAsync(int id);
    public Task<IEnumerable<TurmaListaDTO>> ObterPorTipoAsync(TipoCurso tipo);
    public Task<IEnumerable<TurmaListaDTO>> ObterComVagasAsync();
}
