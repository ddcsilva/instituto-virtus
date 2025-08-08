namespace InstitutoVirtus.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public string? CriadoPor { get; protected set; }
    public string? AtualizadoPor { get; protected set; }

    public void SetCriadoPor(string usuario)
    {
        CriadoPor = usuario;
    }

    public void SetAtualizadoPor(string usuario)
    {
        AtualizadoPor = usuario;
        DataAtualizacao = DateTime.UtcNow;
    }
}