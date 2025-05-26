namespace Virtus.Domain.Tests.Builders;

public class MatriculaBuilder
{
    private Aluno _aluno = AlunoBuilder.Novo();
    private Turma _turma = TurmaBuilder.Nova();
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
        var matricula = new Matricula(_aluno, _turma);

        switch (_status)
        {
            case StatusMatricula.Cancelada:
                matricula.Cancelar();
                break;
            case StatusMatricula.Trancada:
                matricula.Trancar();
                break;
        }

        return matricula;
    }

    public static implicit operator Matricula(MatriculaBuilder builder) => builder.Build();
}
