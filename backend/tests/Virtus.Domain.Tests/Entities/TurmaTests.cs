using FluentAssertions;
using Virtus.Domain.Entities;
using Virtus.Domain.Enums;
using Virtus.Domain.Exceptions;
using Xunit;

namespace Virtus.Domain.Tests.Entities;

public class TurmaTests
{
  private Professor CriarProfessor()
  {
    var pessoa = new Pessoa("Prof. Carlos", TipoPessoa.Professor);
    return new Professor(pessoa, "Violão");
  }

  [Fact]
  public void Construtor_DadosValidos_DeveCriarTurma()
  {
    // Arrange
    var professor = CriarProfessor();
    var dataInicio = DateTime.UtcNow.AddDays(7);

    // Act
    var turma = new Turma("Violão Básico", 10, TipoCurso.Violao,
      professor, "19:00", "Segunda", dataInicio);

    // Assert
    turma.Nome.Should().Be("Violão Básico");
    turma.Capacidade.Should().Be(10);
    turma.Tipo.Should().Be(TipoCurso.Violao);
    turma.ProfessorId.Should().Be(professor.Id);
    turma.Ativa.Should().BeTrue();
  }

  [Fact]
  public void TemVagasDisponiveis_TurmaVazia_DeveRetornarTrue()
  {
    // Arrange
    var professor = CriarProfessor();
    var turma = new Turma("Violão Básico", 2, TipoCurso.Violao,
      professor, "19:00", "Segunda", DateTime.UtcNow);

    // Act
    var result = turma.TemVagasDisponiveis();

    // Assert
    result.Should().BeTrue();
    turma.VagasDisponiveis().Should().Be(2);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  [InlineData(51)]
  public void DefinirCapacidade_CapacidadeInvalida_DeveLancarExcecao(int capacidadeInvalida)
  {
    // Arrange
    var professor = CriarProfessor();
    var turma = new Turma("Violão Básico", 10, TipoCurso.Violao,
      professor, "19:00", "Segunda", DateTime.UtcNow);

    // Act
    var act = () => turma.DefinirCapacidade(capacidadeInvalida);

    // Assert
    act.Should().Throw<ValidationException>();
  }

  [Fact]
  public void Desativar_TurmaComAlunosAtivos_DeveLancarExcecao()
  {
    // Arrange
    var professor = CriarProfessor();
    var turma = new Turma("Violão Básico", 10, TipoCurso.Violao,
      professor, "19:00", "Segunda", DateTime.UtcNow);

    var pessoaAluno = new Pessoa("João Silva", TipoPessoa.Aluno);
    var aluno = new Aluno(pessoaAluno);
    var matricula = new Matricula(aluno, turma);

    // Act
    var act = () => turma.Desativar();

    // Assert
    act.Should().Throw<BusinessRuleException>()
      .WithMessage("Não é possível desativar turma com alunos matriculados");
  }
}