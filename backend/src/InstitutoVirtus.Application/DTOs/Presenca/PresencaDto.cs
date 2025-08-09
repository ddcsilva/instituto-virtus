namespace InstitutoVirtus.Application.DTOs.Presenca;

public class PresencaDto
{
    public Guid Id { get; set; }
    public Guid AulaId { get; set; }
    public Guid AlunoId { get; set; }
    public string AlunoNome { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Justificativa { get; set; }
}

public class RegistrarPresencaDto
{
    public Guid AlunoId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Justificativa { get; set; }
}
