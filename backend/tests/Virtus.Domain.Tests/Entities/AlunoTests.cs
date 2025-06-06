using FluentAssertions;
using Virtus.Domain.Entities;
using Virtus.Domain.Enums;
using Virtus.Domain.Exceptions;
using Xunit;

namespace Virtus.Domain.Tests.Entities;

public class AlunoTests
{
  [Fact]
  public void Construtor_DadosValidos_DeveCriarAluno()
  {
    // Arrange
    var pessoa = new Pessoa("João Silva", TipoPessoa.Aluno);

    // Act
    var aluno = new Aluno(pessoa);

    // Assert
    aluno.PessoaId.Should().Be(pessoa.Id);
    aluno.Status.Should().Be(StatusAluno.Ativo);
    aluno.ResponsavelId.Should().BeNull();
  }

  [Fact]
  public void Construtor_PessoaNaoTipoAluno_DeveLancarExcecao()
  {
    // Arrange
    var pessoa = new Pessoa("Maria Silva", TipoPessoa.Professor);

    // Act
    var act = () => new Aluno(pessoa);

    // Assert
    act.Should().Throw<BusinessRuleException>()
      .WithMessage("Pessoa deve ser do tipo Aluno");
  }

  [Fact]
  public void PodeMatricular_AlunoAtivoETurmaComVaga_DeveRetornarTrue()
  {
    // Arrange
    var pessoaAluno = new Pessoa("João Silva", TipoPessoa.Aluno);
    var aluno = new Aluno(pessoaAluno);

    var pessoaProfessor = new Pessoa("Prof. Carlos", TipoPessoa.Professor);
    var professor = new Professor(pessoaProfessor, "Violão");
    var turma = new Turma("Violão Básico", 10, TipoCurso.Violao, professor, "19:00", "Segunda", DateTime.UtcNow);

    // Act
    var result = aluno.PodeMatricular(turma);

    // Assert
    result.Should().BeTrue();
  }

  [Fact]
  public void DefinirResponsavel_ResponsavelValido_DeveDefinirResponsavel()
  {
    // Arrange
    var pessoaAluno = new Pessoa("João Silva", TipoPessoa.Aluno);
    var aluno = new Aluno(pessoaAluno);
    var pessoaResponsavel = new Pessoa("Maria Silva", TipoPessoa.Responsavel);

    // Act
    aluno.DefinirResponsavel(pessoaResponsavel);

    // Assert
    aluno.ResponsavelId.Should().Be(pessoaResponsavel.Id);
    aluno.Responsavel.Should().Be(pessoaResponsavel);
  }
}