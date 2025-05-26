namespace Virtus.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime DataAtualizacao { get; private set; }

    protected BaseEntity()
    {
        DataCriacao = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    protected void AtualizarData()
    {
        DataAtualizacao = DateTime.UtcNow;
    }
}
