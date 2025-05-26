namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa uma matrícula.
/// </summary>
public class Matricula
{
    public int Id { get; set; }
    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; } = default!;
    public int TurmaId { get; set; }
    public Turma Turma { get; set; } = default!;
    public Enums.StatusMatricula Status { get; set; }
    public DateTime DataMatricula { get; set; }
    public DateTime? DataCancelamento { get; set; }

    protected Matricula() { }

    public Matricula(Aluno aluno, Turma turma)
    {
        Aluno = aluno ?? throw new ArgumentNullException(nameof(aluno));
        AlunoId = aluno.Id;

        Turma = turma ?? throw new ArgumentNullException(nameof(turma));
        TurmaId = turma.Id;

        if (!aluno.PodeMatricular(turma))
        {
            throw new InvalidOperationException("Aluno não pode ser matriculado nesta turma");
        }

        Status = Enums.StatusMatricula.Ativa;
        DataMatricula = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancela a matrícula.
    /// </summary>
    public void Cancelar()
    {
        if (Status == Enums.StatusMatricula.Cancelada)
        {
            throw new InvalidOperationException("Matrícula já está cancelada");
        }

        Status = Enums.StatusMatricula.Cancelada;
        DataCancelamento = DateTime.UtcNow;
    }

    /// <summary>
    /// Tranca a matrícula.
    /// </summary>
    public void Trancar()
    {
        if (Status != Enums.StatusMatricula.Ativa)
        {
            throw new InvalidOperationException("Apenas matrículas ativas podem ser trancadas");
        }

        Status = Enums.StatusMatricula.Trancada;
    }

    /// <summary>
    /// Reativa a matrícula.
    /// </summary>
    public void Reativar()
    {
        if (Status == Enums.StatusMatricula.Ativa)
        {
            throw new InvalidOperationException("Matrícula já está ativa");
        }

        Status = Enums.StatusMatricula.Ativa;
        DataCancelamento = null;
    }
}
