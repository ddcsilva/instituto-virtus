using InstitutoVirtus.Domain.Common;
using InstitutoVirtus.Domain.Enums;

namespace InstitutoVirtus.Domain.Entities;

public class Aula : AuditableEntity
{
  private readonly List<Presenca> _presencas = new();

  public Guid TurmaId { get; private set; }
  public DateTime DataAula { get; private set; }
  public string? Conteudo { get; private set; }
  public string? Observacoes { get; private set; }
  public bool Realizada { get; private set; }

  public virtual Turma? Turma { get; private set; }
  public IReadOnlyCollection<Presenca> Presencas => _presencas;

  protected Aula() { }

  public Aula(Guid turmaId, DateTime dataAula, string? conteudo = null)
  {
    TurmaId = turmaId;
    DataAula = dataAula;
    Conteudo = conteudo;
    Realizada = false;
  }

  public void MarcarComoRealizada()
  {
    Realizada = true;
  }

  public void RegistrarPresenca(Guid alunoId, StatusPresenca status)
  {
    var presencaExistente = _presencas.FirstOrDefault(p => p.AlunoId == alunoId);

    if (presencaExistente != null)
    {
      presencaExistente.AtualizarStatus(status);
    }
    else
    {
      var presenca = new Presenca(Id, alunoId, status);
      _presencas.Add(presenca);
    }
  }

  public double CalcularPercentualPresenca()
  {
    if (!_presencas.Any())
      return 0;

    var presentes = _presencas.Count(p => p.Status == StatusPresenca.Presente || p.Status == StatusPresenca.Justificada);
    return (double)presentes / _presencas.Count * 100;
  }
}
