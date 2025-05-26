using Virtus.Domain.Entities;

namespace Virtus.Domain.Interfaces;

public interface IAlunoRepository : IBaseRepository<Aluno>
{
    public Task<Aluno?> ObterComPessoaAsync(int id);
    public Task<IEnumerable<Aluno>> ObterTodosComPessoaAsync();
    public Task<IEnumerable<Aluno>> ObterPorResponsavelAsync(int responsavelId);
    public Task<IEnumerable<Aluno>> ObterPorStatusAsync(Enums.StatusAluno status);
    public Task<bool> ExisteComEmailAsync(string email);
}
