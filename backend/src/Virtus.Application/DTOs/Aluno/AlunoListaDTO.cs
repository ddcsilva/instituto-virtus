namespace Virtus.Application.DTOs.Aluno;

public class AlunoListaDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? NomeResponsavel { get; set; }
    public int QuantidadeTurmas { get; set; }
}
