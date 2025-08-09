namespace InstitutoVirtus.Application.DTOs.Pagamento;

public class PagamentoDto
{
    public Guid Id { get; set; }
    public Guid ResponsavelId { get; set; }
    public string ResponsavelNome { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public DateTime DataPagamento { get; set; }
    public string MeioPagamento { get; set; } = string.Empty;
    public string? ReferenciaExterna { get; set; }
    public string? ComprovanteUrl { get; set; }
    public string? Observacoes { get; set; }
    public List<ParcelaDto> Parcelas { get; set; } = new();
    public decimal ValorDisponivel { get; set; }
}

public class ParcelaDto
{
    public Guid MensalidadeId { get; set; }
    public string Competencia { get; set; } = string.Empty;
    public string AlunoNome { get; set; } = string.Empty;
    public decimal ValorAlocado { get; set; }
}