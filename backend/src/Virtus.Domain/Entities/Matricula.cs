using Virtus.Domain.Enums;

namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa uma matrícula.
/// </summary>
public class Matricula : BaseEntity
{
    public int AlunoId { get; private set; }
    public Aluno Aluno { get; private set; } = default!;
    public int TurmaId { get; private set; }
    public Turma Turma { get; private set; } = default!;
    public StatusMatricula Status { get; private set; }
    public DateTime DataMatricula { get; private set; }
    public DateTime? DataCancelamento { get; private set; }

    private Matricula() { }

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

        Status = StatusMatricula.Ativa;
        DataMatricula = DateTime.UtcNow;
        AtualizarData();
    }

    /// <summary>
    /// Cancela a matrícula.
    /// </summary>
    public void Cancelar()
    {
        if (Status == StatusMatricula.Cancelada)
        {
            throw new InvalidOperationException("Matrícula já está cancelada");
        }

        Status = StatusMatricula.Cancelada;
        DataCancelamento = DateTime.UtcNow;
        AtualizarData();
    }

    /// <summary>
    /// Tranca a matrícula.
    /// </summary>
    public void Trancar()
    {
        if (Status != StatusMatricula.Ativa)
        {
            throw new InvalidOperationException("Apenas matrículas ativas podem ser trancadas");
        }

        Status = StatusMatricula.Trancada;
        AtualizarData();
    }

    /// <summary>
    /// Reativa a matrícula.
    /// </summary>
    public void Reativar()
    {
        if (Status == StatusMatricula.Ativa)
        {
            throw new InvalidOperationException("Matrícula já está ativa");
        }

        Status = StatusMatricula.Ativa;
        DataCancelamento = null;
        AtualizarData();
    }

    /// <summary>
    /// Conclui a matrícula.
    /// </summary>
    public void Concluir()
    {
        if (Status != StatusMatricula.Ativa)
        {
            throw new InvalidOperationException("Apenas matrículas ativas podem ser concluídas");
        }

        Status = StatusMatricula.Concluida;
        AtualizarData();
    }
}
