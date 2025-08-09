using InstitutoVirtus.Domain.Common;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Exceptions;
using InstitutoVirtus.Domain.ValueObjects;

namespace InstitutoVirtus.Domain.Entities;

public class Mensalidade : AuditableEntity
{
    public Guid MatriculaId { get; private set; }
    public Competencia Competencia { get; private set; }
    public Dinheiro Valor { get; private set; }
    public StatusMensalidade Status { get; private set; }
    public DateTime DataVencimento { get; private set; }
    public DateTime? DataPagamento { get; private set; }
    public MeioPagamento? FormaPagamento { get; private set; }
    public string? Observacao { get; private set; }

    public virtual Matricula? Matricula { get; private set; }

    protected Mensalidade() { }

    public Mensalidade(
        Guid matriculaId,
        Competencia competencia,
        Dinheiro valor,
        DateTime dataVencimento)
    {
        MatriculaId = matriculaId;
        Competencia = competencia;
        Valor = valor;
        DataVencimento = dataVencimento;
        Status = StatusMensalidade.EmAberto;

        VerificarVencimento();
    }

    public void RegistrarPagamento(MeioPagamento formaPagamento, DateTime? dataPagamento = null, string? observacao = null)
    {
        if (Status == StatusMensalidade.Pago)
            throw new BusinessRuleValidationException("Mensalidade já está paga");

        Status = StatusMensalidade.Pago;
        DataPagamento = dataPagamento ?? DateTime.UtcNow;
        FormaPagamento = formaPagamento;
        Observacao = observacao;
    }

    public void CancelarPagamento()
    {
        if (Status != StatusMensalidade.Pago)
            throw new BusinessRuleValidationException("Apenas mensalidades pagas podem ser canceladas");

        Status = StatusMensalidade.EmAberto;
        DataPagamento = null;
        FormaPagamento = null;

        VerificarVencimento();
    }

    public void VerificarVencimento()
    {
        if (Status == StatusMensalidade.EmAberto && DateTime.Today > DataVencimento)
        {
            Status = StatusMensalidade.Vencido;
        }
    }

    public bool EstaVencida()
    {
        return Status == StatusMensalidade.Vencido;
    }
}