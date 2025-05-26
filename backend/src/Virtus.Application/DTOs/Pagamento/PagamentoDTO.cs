using Virtus.Application.DTOs.Aluno;

namespace Virtus.Application.DTOs.Pagamento;

public class PagamentoDTO
{
    public int Id { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataPagamento { get; set; }
    public PagadorDTO Pagador { get; set; } = default!;
    public string? Observacao { get; set; }
    public List<AlunoPagamentoDTO> Alunos { get; set; } = new();
    public bool EstaBalanceado { get; set; }
    public DateTime DataCriacao { get; set; }
}
