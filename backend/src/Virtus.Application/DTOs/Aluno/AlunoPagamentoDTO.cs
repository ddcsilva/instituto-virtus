namespace Virtus.Application.DTOs.Aluno;

public class AlunoPagamentoDTO
{
    public int AlunoId { get; set; }
    public string NomeAluno { get; set; } = default!;
    public decimal Valor { get; set; }
}
