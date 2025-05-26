using Virtus.Domain.Enums;

namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa um aluno.
/// </summary>
public class Aluno : BaseEntity
{
    public int PessoaId { get; private set; }
    public Pessoa Pessoa { get; private set; } = default!;
    public int? ResponsavelId { get; private set; }
    public Pessoa? Responsavel { get; private set; }
    public StatusAluno Status { get; private set; } = StatusAluno.Ativo;

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
        AtualizarData();
    }

    /// <summary>
    /// Reativa o aluno.
    /// </summary>
    public void Reativar()
    {
        Status = StatusAluno.Ativo;
        AtualizarData();
    }

    /// <summary>
    /// Desiste do curso.
    /// </summary>
    public void Desistir()
    {
        Status = StatusAluno.Desistente;
        AtualizarData();
    }

    /// <summary>
    /// Adiciona o aluno na lista de espera.
    /// </summary>
    public void AdicionarNaListaDeEspera()
    {
        Status = StatusAluno.ListaEspera;
        AtualizarData();
    }

    /// <summary>
    /// Atualiza o responsável pelo aluno.
    /// </summary>
    /// <param name="responsavel">O novo responsável ou null para remover.</param>
    public void AtualizarResponsavel(Pessoa? responsavel)
    {
        Responsavel = responsavel;
        ResponsavelId = responsavel?.Id;
        AtualizarData();
    }
}
