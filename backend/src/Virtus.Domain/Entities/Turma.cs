using Virtus.Domain.Enums;
using Virtus.Domain.Exceptions;

namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa uma turma do instituto
/// </summary>
public class Turma : BaseEntity
{
  public string Nome { get; private set; } = default!;
  public int Capacidade { get; private set; }
  public TipoCurso Tipo { get; private set; }
  public int ProfessorId { get; private set; }
  public virtual Professor Professor { get; private set; } = default!;
  public string? Descricao { get; private set; }
  public string Horario { get; private set; } = default!;
  public string DiaSemana { get; private set; } = default!;
  public bool Ativa { get; private set; }
  public DateTime DataInicio { get; private set; }
  public DateTime? DataTermino { get; private set; }

  // Relacionamentos
  private readonly List<Matricula> _matriculas = new();
  public virtual IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

  protected Turma() { }

  public Turma(
    string nome,
    int capacidade,
    TipoCurso tipo,
    Professor professor,
    string horario,
    string diaSemana,
    DateTime dataInicio)
  {
    DefinirNome(nome);
    DefinirCapacidade(capacidade);
    Tipo = tipo;
    DefinirProfessor(professor);
    DefinirHorario(horario);
    DefinirDiaSemana(diaSemana);
    DataInicio = dataInicio;
    Ativa = true;
  }

  /// <summary>
  /// Define o nome da turma
  /// </summary>
  public void DefinirNome(string nome)
  {
    if (string.IsNullOrWhiteSpace(nome))
      throw new ValidationException("Nome da turma é obrigatório");

    if (nome.Length < 3)
      throw new ValidationException("Nome da turma deve ter pelo menos 3 caracteres");

    if (nome.Length > 100)
      throw new ValidationException("Nome da turma não pode ter mais de 100 caracteres");

    Nome = nome.Trim();
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Define a capacidade da turma
  /// </summary>
  public void DefinirCapacidade(int capacidade)
  {
    if (capacidade < 1)
      throw new ValidationException("Capacidade deve ser maior que zero");

    if (capacidade > 50)
      throw new ValidationException("Capacidade não pode ser maior que 50 alunos");

    // Não pode reduzir capacidade abaixo do número de alunos matriculados
    var alunosMatriculados = _matriculas.Count(m => m.Status == StatusMatricula.Ativa);
    if (capacidade < alunosMatriculados)
      throw new BusinessRuleException($"Capacidade não pode ser menor que o número de alunos matriculados ({alunosMatriculados})");

    Capacidade = capacidade;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Define o professor da turma
  /// </summary>
  public void DefinirProfessor(Professor professor)
  {
    if (professor is null)
      throw new ValidationException("Professor é obrigatório");

    if (!professor.Ativo)
      throw new BusinessRuleException("Professor deve estar ativo");

    if (!professor.PodeLecionar(Tipo))
      throw new BusinessRuleException($"Professor não pode lecionar {Tipo}");

    // Remove da lista do professor anterior
    Professor?.RemoverTurma(this);

    ProfessorId = professor.Id;
    Professor = professor;

    // Adiciona na lista do novo professor
    professor.AdicionarTurma(this);

    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Define o horário da turma
  /// </summary>
  public void DefinirHorario(string horario)
  {
    if (string.IsNullOrWhiteSpace(horario))
      throw new ValidationException("Horário é obrigatório");

    Horario = horario.Trim();
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Define o dia da semana da turma
  /// </summary>
  public void DefinirDiaSemana(string diaSemana)
  {
    if (string.IsNullOrWhiteSpace(diaSemana))
      throw new ValidationException("Dia da semana é obrigatório");

    var diasValidos = new[] { "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado", "Domingo" };
    if (!diasValidos.Contains(diaSemana, StringComparer.OrdinalIgnoreCase))
      throw new ValidationException("Dia da semana inválido");

    DiaSemana = diaSemana;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Define a descrição da turma
  /// </summary>
  public void DefinirDescricao(string? descricao)
  {
    Descricao = descricao?.Trim();
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Verifica se a turma tem vagas disponíveis
  /// </summary>
  public bool TemVagasDisponiveis()
  {
    if (!Ativa) return false;

    var matriculasAtivas = _matriculas.Count(m => m.Status == StatusMatricula.Ativa);
    return matriculasAtivas < Capacidade;
  }

  /// <summary>
  /// Retorna o número de vagas disponíveis
  /// </summary>
  public int VagasDisponiveis()
  {
    var matriculasAtivas = _matriculas.Count(m => m.Status == StatusMatricula.Ativa);
    return Math.Max(0, Capacidade - matriculasAtivas);
  }

  /// <summary>
  /// Ativa a turma
  /// </summary>
  public void Ativar()
  {
    Ativa = true;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Desativa a turma
  /// </summary>
  public void Desativar()
  {
    // Não pode desativar turma com alunos ativos
    if (_matriculas.Any(m => m.Status == StatusMatricula.Ativa))
      throw new BusinessRuleException("Não é possível desativar turma com alunos matriculados");

    Ativa = false;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Finaliza a turma
  /// </summary>
  public void Finalizar(DateTime dataTermino)
  {
    if (dataTermino < DataInicio)
      throw new ValidationException("Data de término não pode ser anterior à data de início");

    DataTermino = dataTermino;
    Ativa = false;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Adiciona uma matrícula à turma
  /// </summary>
  internal void AdicionarMatricula(Matricula matricula)
  {
    if (matricula is null)
      throw new ValidationException("Matrícula não pode ser nula");

    _matriculas.Add(matricula);
  }
}