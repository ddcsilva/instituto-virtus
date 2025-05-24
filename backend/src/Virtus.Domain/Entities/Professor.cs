namespace Virtus.Domain.Entities;

public class Professor
{
    public int Id { get; set; }
    public int PessoaId { get; set; }
    public Pessoa Pessoa { get; set; } = default!;
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; }

    private readonly List<Turma> _turmas = new();
    public IReadOnlyCollection<Turma> Turmas => _turmas.AsReadOnly();

    protected Professor() { }

    public Professor(Pessoa pessoa)
    {
        Pessoa = pessoa ?? throw new ArgumentNullException(nameof(pessoa));
        PessoaId = pessoa.Id;
        Ativo = true;
        DataCriacao = DateTime.UtcNow;
    }

    public void Ativar()
    {
        Ativo = true;
    }

    public void Inativar()
    {
        Ativo = false;
    }
}
