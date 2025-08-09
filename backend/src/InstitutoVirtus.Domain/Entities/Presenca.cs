using InstitutoVirtus.Domain.Common;
using InstitutoVirtus.Domain.Enums;

namespace InstitutoVirtus.Domain.Entities;

public class Presenca : AuditableEntity
{
    public Guid AulaId { get; private set; }
    public Guid AlunoId { get; private set; }
    public StatusPresenca Status { get; private set; }
    public string? Justificativa { get; private set; }

    public virtual Aula? Aula { get; private set; }
    public virtual Aluno? Aluno { get; private set; }

    protected Presenca() { }

    public Presenca(Guid aulaId, Guid alunoId, StatusPresenca status, string? justificativa = null)
    {
        AulaId = aulaId;
        AlunoId = alunoId;
        Status = status;

        if (status == StatusPresenca.Justificada && string.IsNullOrWhiteSpace(justificativa))
            throw new ArgumentException("Justificativa é obrigatória para falta justificada");

        Justificativa = justificativa;
    }

    public void AtualizarStatus(StatusPresenca status, string? justificativa = null)
    {
        if (status == StatusPresenca.Justificada && string.IsNullOrWhiteSpace(justificativa))
            throw new ArgumentException("Justificativa é obrigatória para falta justificada");

        Status = status;
        Justificativa = justificativa;
    }
}