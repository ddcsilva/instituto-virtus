namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa a relação entre pagamento e aluno
/// </summary>
public class PagamentoAluno : BaseEntity
{
  public int PagamentoId { get; private set; }
  public virtual Pagamento Pagamento { get; private set; } = default!;
  public int AlunoId { get; private set; }
  public virtual Aluno Aluno { get; private set; } = default!;
  public decimal ValorProporcional { get; private set; }
  public string? Observacao { get; private set; }

  protected PagamentoAluno() { }

  public PagamentoAluno(Pagamento pagamento, Aluno aluno, decimal valorProporcional)
  {
    PagamentoId = pagamento.Id;
    Pagamento = pagamento;
    AlunoId = aluno.Id;
    Aluno = aluno;
    ValorProporcional = valorProporcional;
  }

  /// <summary>
  /// Adiciona uma observação ao pagamento do aluno
  /// </summary>
  public void DefinirObservacao(string observacao)
  {
    Observacao = observacao?.Trim();
    DefinirDataAtualizacao();
  }
}