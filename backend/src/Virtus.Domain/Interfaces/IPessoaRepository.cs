using Virtus.Domain.Entities;

namespace Virtus.Domain.Interfaces;

public interface IPessoaRepository : IBaseRepository<Pessoa>
{
    public Task<Pessoa?> ObterPorEmailAsync(string email);
    public Task<bool> EmailJaExisteAsync(string email, int? excluirId = null);
}
