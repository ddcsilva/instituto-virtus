using Virtus.Application.DTOs.Responsavel;

namespace Virtus.Application.DTOs.Aluno;

public class AlunoDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Telefone { get; set; }
    public string Status { get; set; } = default!;
    public ResponsavelDTO? Responsavel { get; set; }
    public DateTime DataCriacao { get; set; }
}
