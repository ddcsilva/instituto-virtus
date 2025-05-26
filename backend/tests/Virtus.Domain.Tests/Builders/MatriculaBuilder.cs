namespace Virtus.Domain.Tests.Builders;

public class MatriculaBuilder
{
    private Aluno? _aluno;
    private Turma? _turma;
    private StatusMatricula _status = StatusMatricula.Ativa;

    public static MatriculaBuilder Nova() => new();

    public MatriculaBuilder ComAluno(Aluno aluno)
    {
        _aluno = aluno;
        return this;
    }

    public MatriculaBuilder ComTurma(Turma turma)
    {
        _turma = turma;
        return this;
    }

    public MatriculaBuilder ComStatus(StatusMatricula status)
    {
        _status = status;
        return this;
    }

    public Matricula Build()
    {
        // Criar novos aluno e turma se não foram especificados
        var aluno = _aluno ?? AlunoBuilder.Novo().Build();
        var turma = _turma ?? TurmaBuilder.Nova().Build();

        var matricula = new Matricula(aluno, turma);

        switch (_status)
        {
            case StatusMatricula.Cancelada:
                matricula.Cancelar();
                break;
            case StatusMatricula.Trancada:
                matricula.Trancar();
                break;
            case StatusMatricula.Concluida:
                matricula.Concluir();
                break;
        }

        return matricula;
    }

    public static implicit operator Matricula(MatriculaBuilder builder) => builder.Build();
}
