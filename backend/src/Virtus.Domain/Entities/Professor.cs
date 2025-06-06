using Virtus.Domain.Enums;
using Virtus.Domain.Exceptions;

namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa um professor do instituto
/// </summary>
public class Professor : BaseEntity
{
  public int PessoaId { get; private set; }
  public virtual Pessoa Pessoa { get; private set; } = default!;
  public string Especialidade { get; private set; } = default!;
  public DateTime DataAdmissao { get; private set; }
  public bool Ativo { get; private set; }

  // Relacionamentos
  private readonly List<Turma> _turmas = new();
  public virtual IReadOnlyCollection<Turma> Turmas => _turmas.AsReadOnly();

  protected Professor() { }

  public Professor(Pessoa pessoa, string especialidade)
  {
    if (pessoa is null)
      throw new ValidationException("Pessoa é obrigatória para criar um professor");

    if (pessoa.Tipo != TipoPessoa.Professor)
      throw new BusinessRuleException("Pessoa deve ser do tipo Professor");

    DefinirEspecialidade(especialidade);

    PessoaId = pessoa.Id;
    Pessoa = pessoa;
    DataAdmissao = DateTime.UtcNow;
    Ativo = true;
  }

  /// <summary>
  /// Define a especialidade do professor
  /// </summary>
  public void DefinirEspecialidade(string especialidade)
  {
    if (string.IsNullOrWhiteSpace(especialidade))
      throw new ValidationException("Especialidade é obrigatória");

    if (especialidade.Length > 100)
      throw new ValidationException("Especialidade não pode ter mais de 100 caracteres");

    Especialidade = especialidade.Trim();
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Ativa o professor
  /// </summary>
  public void Ativar()
  {
    Ativo = true;
    Pessoa.Ativar();
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Desativa o professor
  /// </summary>
  public void Desativar()
  {
    Ativo = false;
    Pessoa.Desativar();
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Verifica se o professor pode lecionar um tipo de curso
  /// </summary>
  public bool PodeLecionar(TipoCurso tipoCurso)
  {
    if (!Ativo) return false;

    return tipoCurso switch
    {
      TipoCurso.Violao => Especialidade.Contains("Violão", StringComparison.OrdinalIgnoreCase),
      TipoCurso.Teclado => Especialidade.Contains("Teclado", StringComparison.OrdinalIgnoreCase) ||
                           Especialidade.Contains("Piano", StringComparison.OrdinalIgnoreCase),
      TipoCurso.Canto => Especialidade.Contains("Canto", StringComparison.OrdinalIgnoreCase) ||
                         Especialidade.Contains("Vocal", StringComparison.OrdinalIgnoreCase),
      TipoCurso.Teologia => Especialidade.Contains("Teologia", StringComparison.OrdinalIgnoreCase),
      _ => false
    };
  }

  /// <summary>
  /// Adiciona uma turma ao professor
  /// </summary>
  internal void AdicionarTurma(Turma turma)
  {
    if (turma is null)
      throw new ValidationException("Turma não pode ser nula");

    _turmas.Add(turma);
  }

  /// <summary>
  /// Remove uma turma do professor
  /// </summary>
  internal void RemoverTurma(Turma turma)
  {
    _turmas.Remove(turma);
  }
}