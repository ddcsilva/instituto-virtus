namespace Virtus.Domain.Tests.Builders;

public class PagamentoBuilder
{
    private decimal _valor = 100.00m;
    private DateTime _dataPagamento = DateTime.UtcNow;
    private Pessoa _pagador = PessoaBuilder.Novo();
    private string? _observacao;

    public static PagamentoBuilder Novo() => new();

    public PagamentoBuilder ComValor(decimal valor)
    {
        _valor = valor;
        return this;
    }

    public PagamentoBuilder ComDataPagamento(DateTime dataPagamento)
    {
        _dataPagamento = dataPagamento;
        return this;
    }

    public PagamentoBuilder ComPagador(Pessoa pagador)
    {
        _pagador = pagador;
        return this;
    }

    public PagamentoBuilder ComObservacao(string observacao)
    {
        _observacao = observacao;
        return this;
    }

    public Pagamento Build()
    {
        return new Pagamento(_valor, _dataPagamento, _pagador, _observacao);
    }

    public static implicit operator Pagamento(PagamentoBuilder builder) => builder.Build();
}
