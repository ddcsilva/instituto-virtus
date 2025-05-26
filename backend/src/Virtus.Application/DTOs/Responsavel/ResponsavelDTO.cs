namespace Virtus.Application.DTOs.Responsavel;

public class ResponsavelDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Telefone { get; set; }
}
