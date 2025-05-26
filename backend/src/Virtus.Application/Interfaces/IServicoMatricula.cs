using Virtus.Application.DTOs.Matricula;
using Virtus.Application.DTOs.Common;

namespace Virtus.Application.Interfaces;

public interface IServicoMatricula
{
    public Task<ResultadoDTO<MatriculaDTO>> CriarAsync(CriarMatriculaDTO dto);
    public Task<ResultadoDTO<bool>> CancelarAsync(int id);
    public Task<ResultadoDTO<bool>> TrancarAsync(int id);
    public Task<ResultadoDTO<bool>> ReativarAsync(int id);
    public Task<IEnumerable<MatriculaDTO>> ObterPorAlunoAsync(int alunoId);
    public Task<IEnumerable<MatriculaDTO>> ObterPorTurmaAsync(int turmaId);
}
