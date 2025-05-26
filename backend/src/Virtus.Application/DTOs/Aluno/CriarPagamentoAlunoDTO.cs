using System.ComponentModel.DataAnnotations;

namespace Virtus.Application.DTOs.Aluno;

public class CriarPagamentoAlunoDTO
{
    [Required(ErrorMessage = "Aluno é obrigatório")]
    public int AlunoId { get; set; }

    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, 10000, ErrorMessage = "Valor deve estar entre R$ 0,01 e R$ 10.000")]
    public decimal Valor { get; set; }
}
