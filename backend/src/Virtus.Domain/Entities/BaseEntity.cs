namespace Virtus.Domain.Entities;

/// <summary>
/// Classe base para todas as entidades do domínio
/// </summary>
public abstract class BaseEntity
{
  public int Id { get; protected set; }
  public DateTime CriadoEm { get; protected set; }
  public DateTime? AtualizadoEm { get; protected set; }

  protected BaseEntity()
  {
    CriadoEm = DateTime.UtcNow;
  }

  /// <summary>
  /// Atualiza a data de modificação
  /// </summary>
  protected void DefinirDataAtualizacao()
  {
    AtualizadoEm = DateTime.UtcNow;
  }

  public override bool Equals(object? obj)
  {
    if (obj is not BaseEntity other)
      return false;

    if (ReferenceEquals(this, other))
      return true;

    if (GetType() != other.GetType())
      return false;

    return Id != 0 && other.Id != 0 && Id == other.Id;
  }

  public override int GetHashCode()
  {
    return (GetType().GetHashCode() * 907) + Id.GetHashCode();
  }
}