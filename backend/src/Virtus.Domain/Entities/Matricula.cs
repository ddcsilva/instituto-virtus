using Virtus.Domain.Enums;
using Virtus.Domain.Exceptions;

namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa a matrícula de um aluno em uma turma
/// </summary>
public class Matricula : BaseEntity
{
  public int AlunoId { get; private set; }
  public virtual Aluno Aluno { get; private set; } = default!;
  public int TurmaId { get; private set; }
  public virtual Turma Turma { get; private set; } = default!;
  public StatusMatricula Status { get; private set; }
  public DateTime DataMatricula { get; private set; }
  public DateTime? DataCancelamento { get; private set; }
  public string? MotivoCancelamento { get; private set; }
  public int NumeroOrdemEspera { get; private set; }

  protected Matricula() { }

  public Matricula(Aluno aluno, Turma turma)
  {
    if (aluno is null)
      throw new ValidationException("Aluno é obrigatório");

    if (turma is null)
      throw new ValidationException("Turma é obrigatória");

    if (!aluno.PodeMatricular(turma))
      throw new BusinessRuleException("Aluno não pode ser matriculado nesta turma");

    AlunoId = aluno.Id;
    Aluno = aluno;
    TurmaId = turma.Id;
    Turma = turma;
    DataMatricula = DateTime.UtcNow;

    // Se tem vaga, matricula direto. Senão, vai para lista de espera
    if (turma.TemVagasDisponiveis())
    {
      Status = StatusMatricula.Ativa;
      NumeroOrdemEspera = 0;
    }
    else
    {
      Status = StatusMatricula.Ativa; // Ativa mas em lista de espera
      // Calcula posição na lista de espera
      NumeroOrdemEspera = turma.Matriculas
        .Where(m => m.NumeroOrdemEspera > 0)
        .Select(m => m.NumeroOrdemEspera)
        .DefaultIfEmpty(0)
        .Max() + 1;
    }

    // Adiciona nos relacionamentos
    aluno.AdicionarMatricula(this);
    turma.AdicionarMatricula(this);
  }

  /// <summary>
  /// Cancela a matrícula
  /// </summary>
  public void Cancelar(string motivo)
  {
    if (Status != StatusMatricula.Ativa)
      throw new BusinessRuleException("Apenas matrículas ativas podem ser canceladas");

    if (string.IsNullOrWhiteSpace(motivo))
      throw new ValidationException("Motivo do cancelamento é obrigatório");

    Status = StatusMatricula.Cancelada;
    DataCancelamento = DateTime.UtcNow;
    MotivoCancelamento = motivo.Trim();
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Transfere a matrícula para outra turma
  /// </summary>
  public void Transferir()
  {
    if (Status != StatusMatricula.Ativa)
      throw new BusinessRuleException("Apenas matrículas ativas podem ser transferidas");

    Status = StatusMatricula.Transferida;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Reativa uma matrícula cancelada
  /// </summary>
  public void Reativar()
  {
    if (Status != StatusMatricula.Cancelada)
      throw new BusinessRuleException("Apenas matrículas canceladas podem ser reativadas");

    if (!Turma.TemVagasDisponiveis())
      throw new BusinessRuleException("Turma não tem vagas disponíveis");

    Status = StatusMatricula.Ativa;
    DataCancelamento = null;
    MotivoCancelamento = null;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Verifica se o aluno está na lista de espera
  /// </summary>
  public bool EstaEmListaEspera()
  {
    return Status == StatusMatricula.Ativa && NumeroOrdemEspera > 0;
  }

  /// <summary>
  /// Remove da lista de espera (quando uma vaga abre)
  /// </summary>
  public void RemoverDaListaEspera()
  {
    if (!EstaEmListaEspera())
      throw new BusinessRuleException("Matrícula não está em lista de espera");

    NumeroOrdemEspera = 0;
    DefinirDataAtualizacao();
  }
}