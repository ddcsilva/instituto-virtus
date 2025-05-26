using System.ComponentModel.DataAnnotations;

namespace Virtus.Application.DTOs.Aluno;

public class CriarAlunoDTO
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 100 caracteres")]
    public string Nome { get; set; } = default!;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = default!;

    [Phone(ErrorMessage = "Telefone inválido")]
    public string? Telefone { get; set; }

    public int? ResponsavelId { get; set; }
}
