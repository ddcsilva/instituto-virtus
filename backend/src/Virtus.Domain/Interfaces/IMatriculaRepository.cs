using Virtus.Domain.Entities;

namespace Virtus.Domain.Interfaces;

public interface IMatriculaRepository : IBaseRepository<Matricula>
{
    public Task<IEnumerable<Matricula>> ObterPorAlunoAsync(int alunoId);
    public Task<IEnumerable<Matricula>> ObterPorTurmaAsync(int turmaId);
    public Task<Matricula?> ObterMatriculaAtivaAsync(int alunoId, int turmaId);
    public Task<bool> AlunoJaMatriculadoAsync(int alunoId, int turmaId);
}
