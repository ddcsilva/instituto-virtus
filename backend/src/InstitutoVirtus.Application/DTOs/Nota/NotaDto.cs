namespace InstitutoVirtus.Application.DTOs.Nota;

public class NotaDto
{
    public Guid Id { get; set; }
    public Guid AvaliacaoId { get; set; }
    public string AvaliacaoNome { get; set; } = string.Empty;
    public Guid AlunoId { get; set; }
    public string AlunoNome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string? Observacoes { get; set; }
}

public class LancarNotaDto
{
    public Guid AlunoId { get; set; }
    public decimal Valor { get; set; }
    public string? Observacoes { get; set; }
}