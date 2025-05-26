namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa um professor.
/// </summary>
public class Professor : BaseEntity
{
    public int PessoaId { get; private set; }
    public Pessoa Pessoa { get; private set; } = default!;
    public bool Ativo { get; private set; } = true;

    private readonly List<Turma> _turmas = [];
    public IReadOnlyCollection<Turma> Turmas => _turmas.AsReadOnly();

    private Professor() { }

    public Professor(Pessoa pessoa)
    {
        Pessoa = pessoa ?? throw new ArgumentNullException(nameof(pessoa));
        PessoaId = pessoa.Id;
        Ativo = true;
    }

    /// <summary>
    /// Ativa o professor.
    /// </summary>
    public void Ativar()
    {
        Ativo = true;
        AtualizarData();
    }

    /// <summary>
    /// Inativa o professor.
    /// </summary>
    public void Inativar()
    {
        Ativo = false;
        AtualizarData();
    }
}
