using Virtus.Domain.Entities;
using Virtus.Domain.Enums;

namespace Virtus.Domain.Tests.Builders;

public class ProfessorBuilder
{
    private Pessoa _pessoa = PessoaBuilder.Novo().ComTipo(TipoPessoa.Professor);
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
        var professor = new Professor(_pessoa);

        if (!_ativo)
        {
            professor.Inativar();
        }

        return professor;
    }

    public static implicit operator Professor(ProfessorBuilder builder) => builder.Build();
}
