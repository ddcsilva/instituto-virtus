namespace InstitutoVirtus.Application.DTOs.Matricula;

public class MatriculaDto
{
    public Guid Id { get; set; }
    public Guid AlunoId { get; set; }
    public string AlunoNome { get; set; } = string.Empty;
    public Guid TurmaId { get; set; }
    public string TurmaNome { get; set; } = string.Empty;
    public DateTime DataMatricula { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? DataTrancamento { get; set; }
    public DateTime? DataConclusao { get; set; }
    public string? MotivoSaida { get; set; }
}

public class MatriculaResumoDto
{
    public Guid Id { get; set; }
    public string TurmaNome { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}