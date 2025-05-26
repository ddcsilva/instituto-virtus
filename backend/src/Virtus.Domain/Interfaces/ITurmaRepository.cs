using Virtus.Domain.Entities;

namespace Virtus.Domain.Interfaces;

public interface ITurmaRepository : IBaseRepository<Turma>
{
    public Task<Turma?> ObterComMatriculasAsync(int id);
    public Task<IEnumerable<Turma>> ObterPorProfessorAsync(int professorId);
    public Task<IEnumerable<Turma>> ObterPorTipoAsync(Enums.TipoCurso tipo);
    public Task<IEnumerable<Turma>> ObterAtivasAsync();
    public Task<IEnumerable<Turma>> ObterComVagasDisponiveisAsync();
}
