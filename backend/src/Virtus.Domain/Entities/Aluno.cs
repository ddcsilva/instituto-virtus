using Virtus.Domain.Enums;
using Virtus.Domain.Exceptions;

namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa um aluno do instituto
/// </summary>
public class Aluno : BaseEntity
{
  public int PessoaId { get; private set; }
  public virtual Pessoa Pessoa { get; private set; } = default!;
  public int? ResponsavelId { get; private set; }
  public virtual Pessoa? Responsavel { get; private set; }
  public StatusAluno Status { get; private set; }
  public DateTime DataMatricula { get; private set; }
  public string? Observacoes { get; private set; }

  // Relacionamentos
  private readonly List<Matricula> _matriculas = new();
  public virtual IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

  protected Aluno() { }

  public Aluno(Pessoa pessoa, Pessoa? responsavel = null)
  {
    if (pessoa is null)
      throw new ValidationException("Pessoa é obrigatória para criar um aluno");

    if (pessoa.Tipo != TipoPessoa.Aluno)
      throw new BusinessRuleException("Pessoa deve ser do tipo Aluno");

    PessoaId = pessoa.Id;
    Pessoa = pessoa;
    Status = StatusAluno.Ativo;
    DataMatricula = DateTime.UtcNow;

    if (responsavel is not null)
      DefinirResponsavel(responsavel);
  }

  /// <summary>
  /// Define o responsável financeiro do aluno
  /// </summary>
  public void DefinirResponsavel(Pessoa responsavel)
  {
    if (responsavel is null)
      throw new ValidationException("Responsável não pode ser nulo");

    if (responsavel.Tipo != TipoPessoa.Responsavel)
      throw new BusinessRuleException("Pessoa deve ser do tipo Responsável");

    ResponsavelId = responsavel.Id;
    Responsavel = responsavel;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Remove o responsável financeiro do aluno
  /// </summary>
  public void RemoverResponsavel()
  {
    ResponsavelId = null;
    Responsavel = null;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Verifica se o aluno pode ser matriculado em uma turma
  /// </summary>
  public bool PodeMatricular(Turma turma)
  {
    // Aluno deve estar ativo
    if (Status != StatusAluno.Ativo)
      return false;

    // Turma deve ter vagas disponíveis
    if (!turma.TemVagasDisponiveis())
      return false;

    // Aluno não pode estar já matriculado na mesma turma
    var jaMatriculado = _matriculas.Any(m =>
      m.TurmaId == turma.Id &&
      m.Status == StatusMatricula.Ativa
    );

    return !jaMatriculado;
  }

  /// <summary>
  /// Altera o status do aluno
  /// </summary>
  public void AlterarStatus(StatusAluno novoStatus)
  {
    if (Status == StatusAluno.Desistente && novoStatus == StatusAluno.Ativo)
      throw new BusinessRuleException("Aluno desistente não pode voltar a ser ativo diretamente");

    Status = novoStatus;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Adiciona observações sobre o aluno
  /// </summary>
  public void AdicionarObservacao(string observacao)
  {
    if (string.IsNullOrWhiteSpace(observacao))
      return;

    Observacoes = string.IsNullOrWhiteSpace(Observacoes)
      ? observacao
      : $"{Observacoes}\n{DateTime.Now:dd/MM/yyyy}: {observacao}";

    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Adiciona uma matrícula ao histórico do aluno
  /// </summary>
  internal void AdicionarMatricula(Matricula matricula)
  {
    if (matricula is null)
      throw new ValidationException("Matrícula não pode ser nula");

    _matriculas.Add(matricula);
  }
}