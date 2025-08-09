namespace InstitutoVirtus.Application.DTOs.Avaliacao;

public class AvaliacaoDto
{
    public Guid Id { get; set; }
    public Guid TurmaId { get; set; }
    public string TurmaNome { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal Peso { get; set; }
    public DateTime? DataAplicacao { get; set; }
    public string? Descricao { get; set; }
}