using System.ComponentModel.DataAnnotations;

namespace Virtus.Application.DTOs.Turma;

public class CriarTurmaDTO
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 100 caracteres")]
    public string Nome { get; set; } = default!;

    [Required(ErrorMessage = "Capacidade é obrigatória")]
    [Range(1, 50, ErrorMessage = "Capacidade deve estar entre 1 e 50")]
    public int Capacidade { get; set; }

    [Required(ErrorMessage = "Tipo de curso é obrigatório")]
    public Domain.Enums.TipoCurso Tipo { get; set; }

    [Required(ErrorMessage = "Professor é obrigatório")]
    public int ProfessorId { get; set; }
}
