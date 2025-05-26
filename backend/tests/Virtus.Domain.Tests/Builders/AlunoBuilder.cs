namespace Virtus.Domain.Tests.Builders;

public class AlunoBuilder
{
    private Pessoa? _pessoa;
    private Pessoa? _responsavel;
    private StatusAluno _status = StatusAluno.Ativo;

    public static AlunoBuilder Novo() => new();

    public AlunoBuilder ComPessoa(Pessoa pessoa)
    {
        _pessoa = pessoa;
        return this;
    }

    public AlunoBuilder ComResponsavel(Pessoa responsavel)
    {
        _responsavel = responsavel;
        return this;
    }

    public AlunoBuilder ComStatus(StatusAluno status)
    {
        _status = status;
        return this;
    }

    public AlunoBuilder SemResponsavel()
    {
        _responsavel = null;
        return this;
    }

    public Aluno Build()
    {
        // Criar uma nova pessoa se não foi especificada uma
        var pessoa = _pessoa ?? PessoaBuilder.Novo().Build();

        var aluno = new Aluno(pessoa, _responsavel);

        // Usar reflection para definir o status se não for Ativo
        if (_status != StatusAluno.Ativo)
        {
            var statusProperty = typeof(Aluno).GetProperty("Status");
            statusProperty?.SetValue(aluno, _status);
        }

        return aluno;
    }

    public static implicit operator Aluno(AlunoBuilder builder) => builder.Build();
}
