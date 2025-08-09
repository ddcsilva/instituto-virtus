using InstitutoVirtus.Domain.Common;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Exceptions;
using InstitutoVirtus.Domain.ValueObjects;

namespace InstitutoVirtus.Domain.Entities;

public class Pagamento : AuditableEntity
{
    private readonly List<PagamentoParcela> _parcelas = new();

    public Guid ResponsavelId { get; private set; }
    public Dinheiro ValorTotal { get; private set; }
    public DateTime DataPagamento { get; private set; }
    public MeioPagamento MeioPagamento { get; private set; }
    public string? ReferenciaExterna { get; private set; }
    public string? ComprovanteUrl { get; private set; }
    public string? Observacoes { get; private set; }

    public virtual Responsavel? Responsavel { get; private set; }
    public IReadOnlyCollection<PagamentoParcela> Parcelas => _parcelas;

    protected Pagamento() { }

    public Pagamento(
        Guid responsavelId,
        Dinheiro valorTotal,
        MeioPagamento meioPagamento,
        DateTime? dataPagamento = null,
        string? referenciaExterna = null,
        string? comprovanteUrl = null)
    {
        ResponsavelId = responsavelId;
        ValorTotal = valorTotal;
        DataPagamento = dataPagamento ?? DateTime.UtcNow;
        MeioPagamento = meioPagamento;
        ReferenciaExterna = referenciaExterna;
        ComprovanteUrl = comprovanteUrl;
    }

    public void AlocarParaMensalidade(Guid mensalidadeId, Dinheiro valor)
    {
        if (valor.Valor <= 0)
            throw new ArgumentException("Valor deve ser positivo");

        var totalAlocado = _parcelas.Sum(p => p.ValorAlocado.Valor);
        if (totalAlocado + valor.Valor > ValorTotal.Valor)
            throw new BusinessRuleValidationException("Valor total alocado excede o valor do pagamento");

        var parcela = new PagamentoParcela(this.Id, mensalidadeId, valor);
        _parcelas.Add(parcela);
    }

    public Dinheiro ObterValorDisponivel()
    {
        var totalAlocado = _parcelas.Sum(p => p.ValorAlocado.Valor);
        return new Dinheiro(ValorTotal.Valor - totalAlocado);
    }

    public bool FoiTotalmenteAlocado()
    {
        return ObterValorDisponivel().Valor == 0;
    }
}