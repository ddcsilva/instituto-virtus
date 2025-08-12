using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;

namespace InstitutoVirtus.Domain.Interfaces.Repositories;

public interface IPessoaRepository : IBaseRepository<Pessoa>
{
    Task<Pessoa?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pessoa>> GetByTipoAsync(TipoPessoa tipo, CancellationToken cancellationToken = default);
    Task<bool> ExistsByTelefoneAsync(string telefone, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCpfAsync(string cpf, CancellationToken cancellationToken = default);
    Task<bool> ExistsByTelefoneForOtherAsync(Guid id, string telefone, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCpfForOtherAsync(Guid id, string cpf, CancellationToken cancellationToken = default);
    Task<IEnumerable<Aluno>> GetAlunosByResponsavelAsync(Guid responsavelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pesquisa pessoas com filtros opcionais e paginação.
    /// </summary>
    /// <returns>Tupla contendo a lista paginada e o total sem paginação.</returns>
    Task<(IEnumerable<Pessoa> Items, int TotalCount)> SearchAsync(
        string? nome,
        TipoPessoa? tipo,
        bool? ativo,
        string? cpf,
        string? telefone,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}