using Virtus.Application.DTOs.Aluno;
using Virtus.Application.DTOs.Turma;

namespace Virtus.Application.DTOs.Matricula;

public class MatriculaDTO
{
    public int Id { get; set; }
    public AlunoMatriculaDTO Aluno { get; set; } = default!;
    public TurmaMatriculaDTO Turma { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime DataMatricula { get; set; }
    public DateTime? DataCancelamento { get; set; }
}
