using Virtus.Domain.Entities;
using Virtus.Domain.Enums;
using Virtus.Domain.ValueObjects;

namespace Virtus.Domain.Tests.Builders;

public class PessoaBuilder
{
    private string _nome = "João Silva";
    private Email _email = Email.Criar("joao@email.com");
    private string _telefone = "(11) 99999-9999";
    private TipoPessoa _tipo = TipoPessoa.Aluno;

    public static PessoaBuilder Novo() => new();

    public PessoaBuilder ComNome(string nome)
    {
        _nome = nome;
        return this;
    }

    public PessoaBuilder ComEmail(string email)
    {
        _email = Email.Criar(email);
        return this;
    }

    public PessoaBuilder ComTelefone(string telefone)
    {
        _telefone = telefone;
        return this;
    }

    public PessoaBuilder ComTipo(TipoPessoa tipo)
    {
        _tipo = tipo;
        return this;
    }

    public Pessoa Build()
    {
        return new Pessoa(_nome, _email, _telefone, _tipo);
    }

    public static implicit operator Pessoa(PessoaBuilder builder) => builder.Build();
}
