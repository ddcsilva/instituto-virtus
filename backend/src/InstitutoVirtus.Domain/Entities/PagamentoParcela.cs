using InstitutoVirtus.Domain.Common;
using InstitutoVirtus.Domain.ValueObjects;

namespace InstitutoVirtus.Domain.Entities;

public class PagamentoParcela : BaseEntity
{
    public Guid PagamentoId { get; private set; }
    public Guid MensalidadeId { get; private set; }
    public Dinheiro ValorAlocado { get; private set; }

    public virtual Pagamento? Pagamento { get; private set; }
    public virtual Mensalidade? Mensalidade { get; private set; }

    protected PagamentoParcela() { }

    public PagamentoParcela(Guid pagamentoId, Guid mensalidadeId, Dinheiro valorAlocado)
    {
        PagamentoId = pagamentoId;
        MensalidadeId = mensalidadeId;
        ValorAlocado = valorAlocado;
    }
}