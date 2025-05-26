using Virtus.Domain.Entidades;

namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa um pagamento.
/// </summary>
public class Pagamento : BaseEntity
{
    public decimal Valor { get; private set; }
    public DateTime DataPagamento { get; private set; }
    public int PagadorId { get; private set; }
    public Pessoa Pagador { get; private set; } = default!;
    public string? Observacao { get; private set; }

    private readonly List<PagamentoAluno> _pagamentoAlunos = [];
    public IReadOnlyCollection<PagamentoAluno> PagamentoAlunos => _pagamentoAlunos.AsReadOnly();

    private Pagamento() { }

    public Pagamento(decimal valor, DateTime dataPagamento, Pessoa pagador, string? observacao = null)
    {
        ValidarValor(valor);

        Valor = valor;
        DataPagamento = dataPagamento;
        Pagador = pagador ?? throw new ArgumentNullException(nameof(pagador));
        PagadorId = pagador.Id;
        Observacao = observacao;
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
        if (aluno is null)
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

        if (ObterTotalDistribuido() + valor > Valor)
        {
            throw new InvalidOperationException("Total distribuído excede o valor do pagamento.");
        }

        var pagamentoAluno = new PagamentoAluno(this, aluno, valor);
        _pagamentoAlunos.Add(pagamentoAluno);
        AtualizarData();
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
