namespace InstitutoVirtus.Application.DTOs.Aula;

public class AulaDto
{
    public Guid Id { get; set; }
    public Guid TurmaId { get; set; }
    public string TurmaNome { get; set; } = string.Empty;
    public DateTime DataAula { get; set; }
    public string? Conteudo { get; set; }
    public string? Observacoes { get; set; }
    public bool Realizada { get; set; }
    public int TotalPresentes { get; set; }
    public int TotalFaltas { get; set; }
    public double PercentualPresenca { get; set; }
}