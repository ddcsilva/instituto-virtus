using Virtus.Domain.Enums;

namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa um aluno.
/// </summary>
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

    /// <summary>
    /// Verifica se o aluno pode ser matriculado em uma turma.
    /// </summary>
    /// <param name="turma">A turma a ser verificada.</param>
    /// <returns>true se o aluno pode ser matriculado, false caso contrário.</returns>
    public bool PodeMatricular(Turma turma)
    {
        if (turma is null)
        {
            throw new ArgumentNullException(nameof(turma));
        }

        return Status == StatusAluno.Ativo && turma.TemVagasDisponiveis();
    }

    /// <summary>
    /// Inativa o aluno.
    /// </summary>
    public void Inativar()
    {
        Status = StatusAluno.Inativo;
    }

    /// <summary>
    /// Reativa o aluno.
    /// </summary>
    public void Reativar()
    {
        Status = StatusAluno.Ativo;
    }

    /// <summary>
    /// Adiciona o aluno na lista de espera.
    /// </summary>
    public void AdicionarNaListaDeEspera()
    {
        Status = StatusAluno.ListaEspera;
    }
}
