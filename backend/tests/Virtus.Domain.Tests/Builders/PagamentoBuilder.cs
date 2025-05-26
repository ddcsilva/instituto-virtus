namespace Virtus.Domain.Tests.Builders;

public class PagamentoBuilder
{
    private decimal _valor = FakerExtensions.ValorPagamento();
    private DateTime _dataPagamento = FakerExtensions.DataRecente();
    private Pessoa? _pagador;
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
        // Criar um novo pagador se não foi especificado um
        var pagador = _pagador ?? PessoaBuilder.Novo().Build();

        return new Pagamento(_valor, _dataPagamento, pagador, _observacao);
    }

    public static implicit operator Pagamento(PagamentoBuilder builder) => builder.Build();
}
