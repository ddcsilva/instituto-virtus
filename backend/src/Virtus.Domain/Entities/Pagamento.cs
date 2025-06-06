using Virtus.Domain.Enums;
using Virtus.Domain.Exceptions;

namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa um pagamento no sistema
/// </summary>
public class Pagamento : BaseEntity
{
  public decimal Valor { get; private set; }
  public DateTime DataPagamento { get; private set; }
  public DateTime DataVencimento { get; private set; }
  public int PagadorId { get; private set; }
  public virtual Pessoa Pagador { get; private set; } = default!;
  public StatusPagamento Status { get; private set; }
  public string? Observacao { get; private set; }
  public string? NumeroTransacao { get; private set; }
  public string FormaPagamento { get; private set; } = default!;

  // Relacionamentos
  private readonly List<PagamentoAluno> _pagamentoAlunos = new();
  public virtual IReadOnlyCollection<PagamentoAluno> PagamentoAlunos => _pagamentoAlunos.AsReadOnly();

  protected Pagamento() { }

  public Pagamento(decimal valor, DateTime dataVencimento, Pessoa pagador, string formaPagamento)
  {
    DefinirValor(valor);
    DataVencimento = dataVencimento;
    DefinirPagador(pagador);
    DefinirFormaPagamento(formaPagamento);
    Status = StatusPagamento.Pendente;
    DataPagamento = DateTime.UtcNow;
  }

  /// <summary>
  /// Define o valor do pagamento
  /// </summary>
  public void DefinirValor(decimal valor)
  {
    if (valor <= 0)
      throw new ValidationException("Valor deve ser maior que zero");

    if (valor > 10000)
      throw new ValidationException("Valor não pode ser maior que R$ 10.000,00");

    Valor = valor;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Define o pagador
  /// </summary>
  public void DefinirPagador(Pessoa pagador)
  {
    if (pagador is null)
      throw new ValidationException("Pagador é obrigatório");

    PagadorId = pagador.Id;
    Pagador = pagador;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Define a forma de pagamento
  /// </summary>
  public void DefinirFormaPagamento(string formaPagamento)
  {
    if (string.IsNullOrWhiteSpace(formaPagamento))
      throw new ValidationException("Forma de pagamento é obrigatória");

    var formasValidas = new[] { "PIX", "Transferência", "Dinheiro", "Cartão" };
    if (!formasValidas.Contains(formaPagamento, StringComparer.OrdinalIgnoreCase))
      throw new ValidationException("Forma de pagamento inválida");

    FormaPagamento = formaPagamento;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Adiciona um aluno ao pagamento
  /// </summary>
  public void AdicionarAluno(Aluno aluno, decimal valorProporcional)
  {
    if (aluno is null)
      throw new ValidationException("Aluno é obrigatório");

    if (valorProporcional <= 0)
      throw new ValidationException("Valor proporcional deve ser maior que zero");

    // Verifica se aluno já está no pagamento
    if (_pagamentoAlunos.Any(pa => pa.AlunoId == aluno.Id))
      throw new BusinessRuleException("Aluno já está associado a este pagamento");

    // Valida se a soma dos valores proporcionais não excede o valor total
    var somaProporcional = _pagamentoAlunos.Sum(pa => pa.ValorProporcional) + valorProporcional;
    if (somaProporcional > Valor)
      throw new BusinessRuleException("Soma dos valores proporcionais excede o valor total do pagamento");

    var pagamentoAluno = new PagamentoAluno(this, aluno, valorProporcional);
    _pagamentoAlunos.Add(pagamentoAluno);
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Confirma o pagamento
  /// </summary>
  public void ConfirmarPagamento(string numeroTransacao)
  {
    if (Status != StatusPagamento.Pendente)
      throw new BusinessRuleException("Apenas pagamentos pendentes podem ser confirmados");

    if (string.IsNullOrWhiteSpace(numeroTransacao))
      throw new ValidationException("Número da transação é obrigatório");

    Status = StatusPagamento.Pago;
    NumeroTransacao = numeroTransacao.Trim();
    DataPagamento = DateTime.UtcNow;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Cancela o pagamento
  /// </summary>
  public void Cancelar(string motivo)
  {
    if (Status == StatusPagamento.Pago)
      throw new BusinessRuleException("Pagamentos confirmados não podem ser cancelados");

    if (string.IsNullOrWhiteSpace(motivo))
      throw new ValidationException("Motivo do cancelamento é obrigatório");

    Status = StatusPagamento.Cancelado;
    Observacao = $"Cancelado: {motivo}";
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Marca o pagamento como atrasado
  /// </summary>
  public void MarcarComoAtrasado()
  {
    if (Status != StatusPagamento.Pendente)
      throw new BusinessRuleException("Apenas pagamentos pendentes podem ser marcados como atrasados");

    if (DateTime.UtcNow.Date <= DataVencimento.Date)
      throw new BusinessRuleException("Pagamento ainda não está vencido");

    Status = StatusPagamento.Atrasado;
    DefinirDataAtualizacao();
  }

  /// <summary>
  /// Calcula o valor total associado aos alunos
  /// </summary>
  public decimal ValorTotalDistribuido()
  {
    return _pagamentoAlunos.Sum(pa => pa.ValorProporcional);
  }

  /// <summary>
  /// Verifica se o valor está totalmente distribuído
  /// </summary>
  public bool ValorTotalmenteDistribuido()
  {
    return Math.Abs(Valor - ValorTotalDistribuido()) < 0.01m; // Tolerância para arredondamento
  }
}