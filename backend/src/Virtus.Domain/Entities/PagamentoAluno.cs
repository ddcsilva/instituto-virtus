using Virtus.Domain.Entities;

namespace Virtus.Domain.Entidades;

/// <summary>
/// Entidade que representa um pagamento de aluno.
/// </summary>
public class PagamentoAluno
{
    public int Id { get; set; }
    public int PagamentoId { get; set; }
    public Pagamento Pagamento { get; set; } = default!;
    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; } = default!;
    public decimal Valor { get; set; }

    protected PagamentoAluno() { }

    /// <summary>
    /// Cria uma nova instância de PagamentoAluno.
    /// </summary>
    /// <param name="pagamento">O pagamento associado.</param>
    /// <param name="aluno">O aluno associado.</param>
    /// <param name="valor">O valor do pagamento para o aluno.</param>
    public PagamentoAluno(Pagamento pagamento, Aluno aluno, decimal valor)
    {
        Pagamento = pagamento ?? throw new ArgumentNullException(nameof(pagamento));
        PagamentoId = pagamento.Id;

        Aluno = aluno ?? throw new ArgumentNullException(nameof(aluno));
        AlunoId = aluno.Id;

        if (valor <= 0)
        {
            throw new ArgumentException("Valor deve ser maior que zero");
        }

        Valor = valor;
    }
}
