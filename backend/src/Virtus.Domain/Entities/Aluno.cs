using Virtus.Domain.Enums;

namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa um aluno.
/// </summary>
public class Aluno
{
    public int Id { get; private set; }
    public int PessoaId { get; private set; }
    public Pessoa Pessoa { get; private set; } = default!;
    public int? ResponsavelId { get; private set; }
    public Pessoa? Responsavel { get; private set; }
    public StatusAluno Status { get; private set; } = StatusAluno.Ativo;
    public DateTime DataCriacao { get; private set; }
    public DateTime DataAtualizacao { get; private set; }

    private readonly List<Matricula> _matriculas = [];
    public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

    private Aluno() { }

    public Aluno(Pessoa pessoa, Pessoa? responsavel = null)
    {
        Pessoa = pessoa ?? throw new ArgumentNullException(nameof(pessoa));
        PessoaId = pessoa.Id;
        Responsavel = responsavel;
        ResponsavelId = responsavel?.Id;
        Status = StatusAluno.Ativo;
        DataCriacao = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Verifica se o aluno pode ser matriculado em uma turma.
    /// </summary>
    /// <param name="turma">A turma a ser verificada.</param>
    /// <returns>true se o aluno pode ser matriculado, false caso contrário.</returns>
    public bool PodeMatricular(Turma turma)
    {
        if (turma is not null)
        {
            return Status == StatusAluno.Ativo && turma.TemVagasDisponiveis();
        }

        throw new ArgumentNullException(nameof(turma));
    }

    /// <summary>
    /// Inativa o aluno.
    /// </summary>
    public void Inativar()
    {
        Status = StatusAluno.Inativo;
        DataAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Reativa o aluno.
    /// </summary>
    public void Reativar()
    {
        Status = StatusAluno.Ativo;
        DataAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Desiste do curso.
    /// </summary>
    public void Desistir()
    {
        Status = StatusAluno.Desistente;
        DataAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Adiciona o aluno na lista de espera.
    /// </summary>
    public void AdicionarNaListaDeEspera()
    {
        Status = StatusAluno.ListaEspera;
        DataAtualizacao = DateTime.UtcNow;
    }
}
