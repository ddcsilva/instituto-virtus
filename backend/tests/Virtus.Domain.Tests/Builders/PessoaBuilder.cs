namespace Virtus.Domain.Tests.Builders;

public class PessoaBuilder
{
    private string _nome = FakerExtensions.NomeCompleto();
    private Email _email = Email.Criar(FakerExtensions.Email());
    private string _telefone = FakerExtensions.Telefone();
    private TipoPessoa _tipo = TipoPessoa.Aluno;

    public static PessoaBuilder Novo() => new();

    /// <summary>
    /// Cria um builder com dados determinísticos para testes que precisam de valores específicos
    /// </summary>
    public static PessoaBuilder Deterministic() => new()
    {
        _nome = "João Silva",
        _email = Email.Criar("joao@email.com"),
        _telefone = "(11) 99999-9999",
        _tipo = TipoPessoa.Aluno
    };

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
