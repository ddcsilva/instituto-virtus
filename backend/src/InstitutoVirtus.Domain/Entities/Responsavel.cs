using InstitutoVirtus.Domain.Exceptions;
using InstitutoVirtus.Domain.ValueObjects;
using InstitutoVirtus.Domain.Enums;

namespace InstitutoVirtus.Domain.Entities;

public class Responsavel : Pessoa
{
    private readonly List<ResponsavelAluno> _alunos = new();
    private readonly List<Pagamento> _pagamentos = new();

    public IReadOnlyCollection<ResponsavelAluno> Alunos => _alunos;
    public IReadOnlyCollection<Pagamento> Pagamentos => _pagamentos;
    public decimal SaldoCredito { get; private set; }

    protected Responsavel() { }

    public Responsavel(
        string nomeCompleto,
        Telefone telefone,
        Email email,
        DateTime dataNascimento,
        string? observacoes = null)
        : base(nomeCompleto, telefone, email, dataNascimento, TipoPessoa.Responsavel, observacoes)
    {
        if (email == null)
            throw new ArgumentNullException(nameof(email), "Email é obrigatório para responsável");

        SaldoCredito = 0;
    }

    public void AdicionarCredito(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException("Valor deve ser positivo");

        SaldoCredito += valor;
    }

    public void DescontarCredito(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException("Valor deve ser positivo");

        if (valor > SaldoCredito)
            throw new BusinessRuleValidationException("Saldo insuficiente");

        SaldoCredito -= valor;
    }

    public bool TemAlunosVinculados()
    {
        return _alunos.Any();
    }

    public IEnumerable<Guid> ObterIdsAlunos()
    {
        return _alunos.Select(a => a.AlunoId);
    }
}