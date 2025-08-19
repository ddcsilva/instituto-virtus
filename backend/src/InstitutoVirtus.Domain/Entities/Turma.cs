using InstitutoVirtus.Domain.Common;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.ValueObjects;
using InstitutoVirtus.Domain.Exceptions;

namespace InstitutoVirtus.Domain.Entities;

public class Turma : AuditableEntity
{
    private readonly List<Matricula> _matriculas = new();
    private readonly List<Aula> _aulas = new();
    private readonly List<Avaliacao> _avaliacoes = new();

    public Guid CursoId { get; private set; }
    public Guid ProfessorId { get; private set; }
    public DiaSemana DiaSemana { get; private set; }
    public HorarioAula Horario { get; private set; }
    public int Capacidade { get; private set; }
    public string? Sala { get; private set; }
    public int AnoLetivo { get; private set; }
    public int Periodo { get; private set; } // 1 = 1º semestre, 2 = 2º semestre
    public bool Ativo { get; private set; }

    public virtual Curso? Curso { get; private set; }
    public virtual Professor? Professor { get; private set; }
    public IReadOnlyCollection<Matricula> Matriculas => _matriculas;
    public IReadOnlyCollection<Aula> Aulas => _aulas;
    public IReadOnlyCollection<Avaliacao> Avaliacoes => _avaliacoes;

#pragma warning disable CS8618
    protected Turma() { }
#pragma warning restore CS8618

    public Turma(
        Guid cursoId,
        Guid professorId,
        DiaSemana diaSemana,
        HorarioAula horario,
        int capacidade,
        int anoLetivo,
        int periodo,
        string? sala = null)
    {
        if (capacidade <= 0)
            throw new ArgumentException("Capacidade deve ser maior que zero");

        if (periodo < 1 || periodo > 2)
            throw new ArgumentException("Período deve ser 1 ou 2");

        CursoId = cursoId;
        ProfessorId = professorId;
        DiaSemana = diaSemana;
        Horario = horario;
        Capacidade = capacidade;
        AnoLetivo = anoLetivo;
        Periodo = periodo;
        Sala = sala;
        Ativo = true;
    }

    public string ObterNome()
    {
        var cursoNome = Curso?.Nome ?? string.Empty;
        var profNome = Professor?.NomeCompleto ?? string.Empty;
        var dia = DiaSemana.ToString();
        var horario = Horario.FormatoString();

        var partes = new List<string>();
        if (!string.IsNullOrWhiteSpace(cursoNome)) partes.Add(cursoNome);
        if (!string.IsNullOrWhiteSpace(profNome)) partes.Add(profNome);
        partes.Add($"{dia} {horario}");

        return string.Join(" — ", partes);
    }

    public bool TemVaga()
    {
        return _matriculas.Count(m => m.Status == StatusMatricula.Ativa) < Capacidade;
    }

    public int VagasDisponiveis()
    {
        var matriculasAtivas = _matriculas.Count(m => m.Status == StatusMatricula.Ativa);
        return Math.Max(0, Capacidade - matriculasAtivas);
    }

    public void Ativar() => Ativo = true;
    public void Desativar() => Ativo = false;

    public void AtualizarCapacidade(int novaCapacidade)
    {
        if (novaCapacidade <= 0)
            throw new ArgumentException("Capacidade deve ser maior que zero");

        var matriculasAtivas = _matriculas.Count(m => m.Status == StatusMatricula.Ativa);
        if (novaCapacidade < matriculasAtivas)
            throw new BusinessRuleValidationException(
                "Nova capacidade não pode ser menor que o número de matrículas ativas");

        Capacidade = novaCapacidade;
    }

    public void AtualizarSala(string? sala)
    {
        Sala = sala;
    }
}