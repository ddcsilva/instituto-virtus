using Virtus.Application.DTOs.Aluno;
using Virtus.Application.DTOs.Common;

namespace Virtus.Application.Interfaces;

public interface IServicoAluno
{
    public Task<ResultadoDTO<AlunoDTO>> ObterPorIdAsync(int id);
    public Task<RespostaPaginadaDTO<AlunoListaDTO>> ObterTodosAsync(int pagina = 1, int itensPorPagina = 20);
    public Task<ResultadoDTO<AlunoDTO>> CriarAsync(CriarAlunoDTO dto);
    public Task<ResultadoDTO<AlunoDTO>> AtualizarAsync(int id, AtualizarAlunoDTO dto);
    public Task<ResultadoDTO<bool>> InativarAsync(int id);
    public Task<ResultadoDTO<bool>> ReativarAsync(int id);
    public Task<IEnumerable<AlunoListaDTO>> ObterPorResponsavelAsync(int responsavelId);
    public Task<IEnumerable<AlunoListaDTO>> ObterListaEsperaAsync();
}
