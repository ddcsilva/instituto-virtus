namespace Virtus.Domain.Tests.Builders;

public class ProfessorBuilder
{
    private Pessoa? _pessoa;
    private bool _ativo = true;

    public static ProfessorBuilder Novo() => new();

    public ProfessorBuilder ComPessoa(Pessoa pessoa)
    {
        _pessoa = pessoa;
        return this;
    }

    public ProfessorBuilder Inativo()
    {
        _ativo = false;
        return this;
    }

    public Professor Build()
    {
        // Criar uma nova pessoa se não foi especificada uma
        var pessoa = _pessoa ?? PessoaBuilder.Novo().ComTipo(TipoPessoa.Professor).Build();

        var professor = new Professor(pessoa);

        if (!_ativo)
        {
            professor.Inativar();
        }

        return professor;
    }

    public static implicit operator Professor(ProfessorBuilder builder) => builder.Build();
}
