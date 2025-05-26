namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa um professor.
/// </summary>
public class Professor
{
    public int Id { get; private set; }
    public int PessoaId { get; private set; }
    public Pessoa Pessoa { get; private set; } = default!;
    public bool Ativo { get; private set; } = true;
    public DateTime DataCriacao { get; private set; }
    public DateTime DataAtualizacao { get; private set; }

    private readonly List<Turma> _turmas = [];
    public IReadOnlyCollection<Turma> Turmas => _turmas.AsReadOnly();

    private Professor() { }

    public Professor(Pessoa pessoa)
    {
        Pessoa = pessoa ?? throw new ArgumentNullException(nameof(pessoa));
        PessoaId = pessoa.Id;
        Ativo = true;
        DataCriacao = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Ativa o professor.
    /// </summary>
    public void Ativar()
    {
        Ativo = true;
        DataAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Inativa o professor.
    /// </summary>
    public void Inativar()
    {
        Ativo = false;
        DataAtualizacao = DateTime.UtcNow;
    }
}
