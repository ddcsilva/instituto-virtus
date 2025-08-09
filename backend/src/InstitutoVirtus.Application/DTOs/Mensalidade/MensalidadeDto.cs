namespace InstitutoVirtus.Application.DTOs.Mensalidade;

public class MensalidadeDto
{
    public Guid Id { get; set; }
    public Guid MatriculaId { get; set; }
    public string AlunoNome { get; set; } = string.Empty;
    public string CursoNome { get; set; } = string.Empty;
    public string Competencia { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public string? FormaPagamento { get; set; }
    public string? Observacao { get; set; }
    public bool EstaVencida { get; set; }
}