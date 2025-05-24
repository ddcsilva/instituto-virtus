using Virtus.Domain.Enums;

namespace Virtus.Domain.Entities;

public class Aluno
{
    public int Id { get; set; }
    public int PessoaId { get; set; }
    public Pessoa Pessoa { get; set; } = default!;
    public int? ResponsavelId { get; set; }
    public Pessoa? Responsavel { get; set; }
    public StatusAluno Status { get; set; } = StatusAluno.Ativo;
    public DateTime DataCriacao { get; set; }

    private readonly List<Matricula> _matriculas = new();
    public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

    protected Aluno() { }

    public Aluno(Pessoa pessoa, Pessoa? responsavel = null)
    {
        Pessoa = pessoa ?? throw new ArgumentNullException(nameof(pessoa));
        PessoaId = pessoa.Id;

        if (responsavel != null)
        {
            Responsavel = responsavel;
            ResponsavelId = responsavel.Id;
        }

        Status = StatusAluno.Ativo;
        DataCriacao = DateTime.UtcNow;
    }

    public bool PodeMatricular(Turma turma)
    {
        if (turma is null)
        {
            throw new ArgumentNullException(nameof(turma));
        }

        return Status == StatusAluno.Ativo && turma.TemVagasDisponiveis();
    }

    public void Inativar()
    {
        Status = StatusAluno.Inativo;
    }

    public void Reativar()
    {
        Status = StatusAluno.Ativo;
    }

    public void AdicionarNaListaDeEspera()
    {
        Status = StatusAluno.ListaEspera;
    }
}
