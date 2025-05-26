using Virtus.Application.DTOs.Professor;

namespace Virtus.Application.DTOs.Turma;

public class TurmaDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public int Capacidade { get; set; }
    public string Tipo { get; set; } = default!;
    public ProfessorDTO Professor { get; set; } = default!;
    public int VagasDisponiveis { get; set; }
    public int AlunosMatriculados { get; set; }
    public bool Ativa { get; set; }
    public DateTime DataCriacao { get; set; }
}
