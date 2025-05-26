namespace Virtus.Application.DTOs.Turma;

public class TurmaListaDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public string Tipo { get; set; } = default!;
    public string NomeProfessor { get; set; } = default!;
    public string StatusVagas { get; set; } = default!;
    public bool TemVagas { get; set; }
    public bool Ativa { get; set; }
}
