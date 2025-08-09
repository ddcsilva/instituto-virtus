namespace InstitutoVirtus.Application.DTOs.Turma;

public class TurmaDto
{
    public Guid Id { get; set; }
    public Guid CursoId { get; set; }
    public string CursoNome { get; set; } = string.Empty;
    public Guid ProfessorId { get; set; }
    public string ProfessorNome { get; set; } = string.Empty;
    public string DiaSemana { get; set; } = string.Empty;
    public string HorarioInicio { get; set; } = string.Empty;
    public string HorarioFim { get; set; } = string.Empty;
    public int Capacidade { get; set; }
    public string? Sala { get; set; }
    public int AnoLetivo { get; set; }
    public int Periodo { get; set; }
    public bool Ativo { get; set; }
    public int AlunosMatriculados { get; set; }
    public int VagasDisponiveis { get; set; }
}

public class TurmaResumoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Horario { get; set; } = string.Empty;
    public int VagasDisponiveis { get; set; }
}