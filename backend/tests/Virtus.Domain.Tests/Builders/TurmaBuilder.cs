using Virtus.Domain.Entities;
using Virtus.Domain.Enums;

namespace Virtus.Domain.Tests.Builders;

public class TurmaBuilder
{
    private string _nome = "Turma de Violão";
    private int _capacidade = 10;
    private TipoCurso _tipo = TipoCurso.Violao;
    private Professor _professor = ProfessorBuilder.Novo();
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
        var turma = new Turma(_nome, _capacidade, _tipo, _professor);

        if (!_ativa)
        {
            turma.Inativar();
        }

        return turma;
    }

    public static implicit operator Turma(TurmaBuilder builder) => builder.Build();
}
