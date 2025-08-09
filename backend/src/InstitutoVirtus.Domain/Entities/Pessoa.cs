namespace InstitutoVirtus.Domain.Entities;

using InstitutoVirtus.Domain.Common;
using InstitutoVirtus.Domain.ValueObjects;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Exceptions;

public class Pessoa : AuditableEntity
{
    public string NomeCompleto { get; private set; }
    public Telefone Telefone { get; private set; }
    public Email? Email { get; private set; }
    public DateTime DataNascimento { get; private set; }
    public TipoPessoa TipoPessoa { get; private set; }
    public string? Observacoes { get; private set; }
    public bool Ativo { get; private set; }

    protected Pessoa() { }

    public Pessoa(
        string nomeCompleto,
        Telefone telefone,
        Email? email,
        DateTime dataNascimento,
        TipoPessoa tipoPessoa,
        string? observacoes = null)
    {
        ValidarNome(nomeCompleto);
        ValidarIdade(dataNascimento, tipoPessoa);

        NomeCompleto = nomeCompleto;
        Telefone = telefone ?? throw new ArgumentNullException(nameof(telefone));
        Email = email;
        DataNascimento = dataNascimento;
        TipoPessoa = tipoPessoa;
        Observacoes = observacoes;
        Ativo = true;
    }

    private void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome completo é obrigatório");

        if (nome.Length < 3)
            throw new ArgumentException("Nome deve ter pelo menos 3 caracteres");
    }

    private void ValidarIdade(DateTime dataNascimento, TipoPessoa tipo)
    {
        var idade = CalcularIdade(dataNascimento);

        if (tipo == TipoPessoa.Aluno && idade < 7)
            throw new BusinessRuleValidationException("Aluno deve ter pelo menos 7 anos");

        if (idade > 120)
            throw new ArgumentException("Data de nascimento inválida");
    }

    public int CalcularIdade(DateTime? dataReferencia = null)
    {
        var hoje = dataReferencia ?? DateTime.Today;
        var idade = hoje.Year - DataNascimento.Year;

        if (DataNascimento.Date > hoje.AddYears(-idade))
            idade--;

        return idade;
    }

    public bool EhMenorDeIdade()
    {
        return CalcularIdade() < 18;
    }

    public void AtualizarDados(
        string nomeCompleto,
        Telefone telefone,
        Email? email,
        string? observacoes)
    {
        ValidarNome(nomeCompleto);

        NomeCompleto = nomeCompleto;
        Telefone = telefone ?? throw new ArgumentNullException(nameof(telefone));
        Email = email;
        Observacoes = observacoes;
    }

    public void Ativar() => Ativo = true;
    public void Desativar() => Ativo = false;
}