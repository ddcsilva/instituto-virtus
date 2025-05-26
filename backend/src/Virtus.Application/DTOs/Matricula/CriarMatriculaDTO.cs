using System.ComponentModel.DataAnnotations;

namespace Virtus.Application.DTOs.Matricula;

public class CriarMatriculaDTO
{
    [Required(ErrorMessage = "Aluno é obrigatório")]
    public int AlunoId { get; set; }

    [Required(ErrorMessage = "Turma é obrigatória")]
    public int TurmaId { get; set; }
}
