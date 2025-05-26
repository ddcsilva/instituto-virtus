namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa um pagamento.
/// </summary>
public class Pagamento
{
    public int Id { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataPagamento { get; set; }
    public int PagadorId { get; set; }
    public Pessoa Pagador { get; set; } = default!;
    public string? Observacao { get; set; }
    public DateTime DataCriacao { get; set; }

    private readonly List<PagamentoAluno> _pagamentoAlunos = new();
    public IReadOnlyCollection<PagamentoAluno> PagamentoAlunos => _pagamentoAlunos.AsReadOnly();

    protected Pagamento() { }

    public Pagamento(decimal valor, DateTime dataPagamento, Pessoa pagador, string? observacao = null)
    {
        ValidarValor(valor);

        Valor = valor;
        DataPagamento = dataPagamento;
        Pagador = pagador ?? throw new ArgumentNullException(nameof(pagador));
        PagadorId = pagador.Id;
        Observacao = observacao;
        DataCriacao = DateTime.UtcNow;
    }

    private static void ValidarValor(decimal valor)
    {
        if (valor <= 0)
        {
            throw new ArgumentException("Valor do pagamento deve ser maior que zero");
        }

        if (valor > 10000)
        {
            throw new ArgumentException("Valor do pagamento não pode ser maior que R$ 10.000");
        }
    }

    /// <summary>
    /// Adiciona um aluno ao pagamento.
    /// </summary>
    /// <param name="aluno">O aluno a ser adicionado.</param>
    /// <param name="valor">O valor a ser distribuído para o aluno.</param>
    public void AdicionarAluno(Aluno aluno, decimal valor)
    {
        if (aluno == null)
        {
            throw new ArgumentNullException(nameof(aluno));
        }

        if (valor <= 0)
        {
            throw new ArgumentException("Valor por aluno deve ser maior que zero");
        }

        if (_pagamentoAlunos.Any(pa => pa.AlunoId == aluno.Id))
        {
            throw new InvalidOperationException("Aluno já está incluído neste pagamento");
        }

        var pagamentoAluno = new PagamentoAluno(this, aluno, valor);
        _pagamentoAlunos.Add(pagamentoAluno);
    }

    /// <summary>
    /// Obtém o total distribuído para os alunos.
    /// </summary>
    /// <returns>O total distribuído para os alunos.</returns>
    public decimal ObterTotalDistribuido()
    {
        return _pagamentoAlunos.Sum(pa => pa.Valor);
    }

    /// <summary>
    /// Verifica se o pagamento está balanceado.
    /// </summary>
    /// <returns>true se o pagamento está balanceado, false caso contrário.</returns>
    public bool EstaBalanceado()
    {
        return ObterTotalDistribuido() == Valor;
    }
}
