namespace Virtus.Domain.Tests.Builders;

public class TurmaBuilder
{
    private string _nome = FakerExtensions.NomeTurma();
    private int _capacidade = FakerExtensions.CapacidadeTurma();
    private TipoCurso _tipo = TipoCurso.Violao;
    private Professor? _professor;
    private bool _ativa = true;

    public static TurmaBuilder Nova() => new();

    public TurmaBuilder ComNome(string nome)
    {
        _nome = nome;
        return this;
    }

    public TurmaBuilder ComCapacidade(int capacidade)
    {
        _capacidade = capacidade;
        return this;
    }

    public TurmaBuilder ComTipo(TipoCurso tipo)
    {
        _tipo = tipo;
        return this;
    }

    public TurmaBuilder ComProfessor(Professor professor)
    {
        _professor = professor;
        return this;
    }

    public TurmaBuilder Inativa()
    {
        _ativa = false;
        return this;
    }

    public Turma Build()
    {
        // Criar um novo professor se não foi especificado um
        var professor = _professor ?? ProfessorBuilder.Novo().Build();

        var turma = new Turma(_nome, _capacidade, _tipo, professor);

        if (!_ativa)
        {
            turma.Inativar();
        }

        return turma;
    }

    public static implicit operator Turma(TurmaBuilder builder) => builder.Build();
}
