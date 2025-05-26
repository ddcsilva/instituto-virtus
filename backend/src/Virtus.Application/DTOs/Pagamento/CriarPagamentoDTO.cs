using System.ComponentModel.DataAnnotations;
using Virtus.Application.DTOs.Aluno;

namespace Virtus.Application.DTOs.Pagamento;

public class CriarPagamentoDTO
{
    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, 10000, ErrorMessage = "Valor deve estar entre R$ 0,01 e R$ 10.000")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "Data do pagamento é obrigatória")]
    public DateTime DataPagamento { get; set; }

    [Required(ErrorMessage = "Pagador é obrigatório")]
    public int PagadorId { get; set; }

    [StringLength(500, ErrorMessage = "Observação não pode ter mais de 500 caracteres")]
    public string? Observacao { get; set; }

    [Required(ErrorMessage = "Deve incluir pelo menos um aluno")]
    [MinLength(1, ErrorMessage = "Deve incluir pelo menos um aluno")]
    public List<CriarPagamentoAlunoDTO> Alunos { get; set; } = new();
}
