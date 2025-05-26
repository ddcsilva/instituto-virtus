namespace Virtus.Domain.Tests.Entities;

public class MatriculaTests
{
    [Fact]
    public void Construtor_DeveInicializarCorretamente_QuandoParametrosValidos()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo()
            .ComStatus(StatusAluno.Ativo)
            .Build();

        var turma = TurmaBuilder.Nova()
            .ComCapacidade(10)
            .Build();

        // Act
        var matricula = new Matricula(aluno, turma);

        // Assert
        matricula.Aluno.Should().Be(aluno);
        matricula.AlunoId.Should().Be(aluno.Id);
        matricula.Turma.Should().Be(turma);
        matricula.TurmaId.Should().Be(turma.Id);
        matricula.Status.Should().Be(StatusMatricula.Ativa);
        matricula.DataMatricula.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        matricula.DataCancelamento.Should().BeNull();
        matricula.Id.Should().Be(0);
        matricula.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        matricula.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoAlunoNulo()
    {
        // Arrange
        Aluno aluno = null!;
        var turma = TurmaBuilder.Nova().Build();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Matricula(aluno, turma));
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoTurmaNula()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo().Build();
        Turma turma = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Matricula(aluno, turma));
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoAlunoNaoPodeMatricular()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo()
            .ComStatus(StatusAluno.Inativo) // Aluno inativo não pode matricular
            .Build();

        var turma = TurmaBuilder.Nova()
            .ComCapacidade(10)
            .Build();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new Matricula(aluno, turma));

        exception.Message.Should().Contain("Aluno não pode ser matriculado nesta turma");
    }

    [Fact]
    public void Cancelar_DeveAlterarStatusParaCancelada_QuandoMatriculaAtiva()
    {
        // Arrange
        var matricula = MatriculaBuilder.Nova()
            .ComStatus(StatusMatricula.Ativa)
            .Build();

        var dataAnterior = matricula.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        matricula.Cancelar();

        // Assert
        matricula.Status.Should().Be(StatusMatricula.Cancelada);
        matricula.DataCancelamento.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        matricula.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Fact]
    public void Cancelar_DeveLancarExcecao_QuandoMatriculaJaCancelada()
    {
        // Arrange
        var matricula = MatriculaBuilder.Nova()
            .ComStatus(StatusMatricula.Cancelada)
            .Build();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            matricula.Cancelar());

        exception.Message.Should().Contain("Matrícula já está cancelada");
    }

    [Fact]
    public void Trancar_DeveAlterarStatusParaTrancada_QuandoMatriculaAtiva()
    {
        // Arrange
        var matricula = MatriculaBuilder.Nova()
            .ComStatus(StatusMatricula.Ativa)
            .Build();

        var dataAnterior = matricula.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        matricula.Trancar();

        // Assert
        matricula.Status.Should().Be(StatusMatricula.Trancada);
        matricula.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Theory]
    [InlineData(StatusMatricula.Cancelada)]
    [InlineData(StatusMatricula.Trancada)]
    [InlineData(StatusMatricula.Concluida)]
    public void Trancar_DeveLancarExcecao_QuandoMatriculaNaoAtiva(StatusMatricula status)
    {
        // Arrange
        var matricula = MatriculaBuilder.Nova()
            .ComStatus(status)
            .Build();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            matricula.Trancar());

        exception.Message.Should().Contain("Apenas matrículas ativas podem ser trancadas");
    }

    [Fact]
    public void Reativar_DeveAlterarStatusParaAtiva_QuandoMatriculaCancelada()
    {
        // Arrange
        var matricula = MatriculaBuilder.Nova()
            .ComStatus(StatusMatricula.Cancelada)
            .Build();

        var dataAnterior = matricula.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        matricula.Reativar();

        // Assert
        matricula.Status.Should().Be(StatusMatricula.Ativa);
        matricula.DataCancelamento.Should().BeNull();
        matricula.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Fact]
    public void Reativar_DeveAlterarStatusParaAtiva_QuandoMatriculaTrancada()
    {
        // Arrange
        var matricula = MatriculaBuilder.Nova()
            .ComStatus(StatusMatricula.Trancada)
            .Build();

        // Act
        matricula.Reativar();

        // Assert
        matricula.Status.Should().Be(StatusMatricula.Ativa);
        matricula.DataCancelamento.Should().BeNull();
    }

    [Fact]
    public void Reativar_DeveLancarExcecao_QuandoMatriculaJaAtiva()
    {
        // Arrange
        var matricula = MatriculaBuilder.Nova()
            .ComStatus(StatusMatricula.Ativa)
            .Build();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            matricula.Reativar());

        exception.Message.Should().Contain("Matrícula já está ativa");
    }

    [Fact]
    public void Matricula_DeveHerdarDeBaseEntity_QuandoCriada()
    {
        // Arrange & Act
        var matricula = MatriculaBuilder.Nova().Build();

        // Assert
        matricula.Should().BeAssignableTo<BaseEntity>();
    }

    [Theory]
    [InlineData(StatusAluno.Desistente)]
    [InlineData(StatusAluno.ListaEspera)]
    public void Construtor_DeveLancarExcecao_QuandoAlunoComStatusInvalido(StatusAluno statusAluno)
    {
        // Arrange
        var aluno = AlunoBuilder.Novo()
            .ComStatus(statusAluno)
            .Build();

        var turma = TurmaBuilder.Nova()
            .ComCapacidade(10)
            .Build();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new Matricula(aluno, turma));

        exception.Message.Should().Contain("Aluno não pode ser matriculado nesta turma");
    }

    [Fact]
    public void DataMatricula_DeveSerDefinidaNoConstrutor()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo().Build();
        var turma = TurmaBuilder.Nova().Build();
        var horaAntes = DateTime.UtcNow;

        // Act
        var matricula = new Matricula(aluno, turma);

        // Assert
        matricula.DataMatricula.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        matricula.DataMatricula.Should().BeOnOrAfter(horaAntes);
    }

    [Fact]
    public void FluxoCompleto_DevePermitirTransicoesDeStatus()
    {
        // Arrange
        var matricula = MatriculaBuilder.Nova().Build();

        // Act & Assert - Fluxo: Ativa -> Trancada -> Ativa -> Cancelada -> Ativa -> Concluida
        matricula.Status.Should().Be(StatusMatricula.Ativa);

        matricula.Trancar();
        matricula.Status.Should().Be(StatusMatricula.Trancada);

        matricula.Reativar();
        matricula.Status.Should().Be(StatusMatricula.Ativa);

        matricula.Cancelar();
        matricula.Status.Should().Be(StatusMatricula.Cancelada);
        matricula.DataCancelamento.Should().NotBeNull();

        matricula.Reativar();
        matricula.Status.Should().Be(StatusMatricula.Ativa);
        matricula.DataCancelamento.Should().BeNull();

        matricula.Concluir();
        matricula.Status.Should().Be(StatusMatricula.Concluida);
    }

    [Fact]
    public void AlunoId_DeveCorresponderAoAlunoAssociado()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo().Build();
        var turma = TurmaBuilder.Nova().Build();

        // Simular que o aluno tem um ID (normalmente vem do banco)
        var alunoIdProperty = typeof(BaseEntity).GetProperty("Id");
        alunoIdProperty?.SetValue(aluno, 456);

        // Act
        var matricula = new Matricula(aluno, turma);

        // Assert
        matricula.AlunoId.Should().Be(aluno.Id);
        matricula.Aluno.Should().Be(aluno);
    }

    [Fact]
    public void TurmaId_DeveCorresponderATurmaAssociada()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo().Build();
        var turma = TurmaBuilder.Nova().Build();

        // Simular que a turma tem um ID (normalmente vem do banco)
        var turmaIdProperty = typeof(BaseEntity).GetProperty("Id");
        turmaIdProperty?.SetValue(turma, 789);

        // Act
        var matricula = new Matricula(aluno, turma);

        // Assert
        matricula.TurmaId.Should().Be(turma.Id);
        matricula.Turma.Should().Be(turma);
    }

    [Fact]
    public void DataCancelamento_DeveSerNulaQuandoMatriculaAtiva()
    {
        // Arrange & Act
        var matricula = MatriculaBuilder.Nova()
            .ComStatus(StatusMatricula.Ativa)
            .Build();

        // Assert
        matricula.DataCancelamento.Should().BeNull();
    }

    [Fact]
    public void Cancelar_DeveDefinirDataCancelamento_QuandoChamado()
    {
        // Arrange
        var matricula = MatriculaBuilder.Nova().Build();

        // Act
        matricula.Cancelar();

        // Assert
        matricula.DataCancelamento.Should().NotBeNull();
        matricula.DataCancelamento.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Trancar_NaoDeveAlterarDataCancelamento()
    {
        // Arrange
        var matricula = MatriculaBuilder.Nova().Build();

        // Act
        matricula.Trancar();

        // Assert
        matricula.DataCancelamento.Should().BeNull();
    }

    [Fact]
    public void Concluir_DeveAlterarStatusParaConcluida_QuandoMatriculaAtiva()
    {
        // Arrange
        var matricula = MatriculaBuilder.Nova()
            .ComStatus(StatusMatricula.Ativa)
            .Build();

        var dataAnterior = matricula.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        matricula.Concluir();

        // Assert
        matricula.Status.Should().Be(StatusMatricula.Concluida);
        matricula.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Theory]
    [InlineData(StatusMatricula.Cancelada)]
    [InlineData(StatusMatricula.Trancada)]
    [InlineData(StatusMatricula.Concluida)]
    public void Concluir_DeveLancarExcecao_QuandoMatriculaNaoAtiva(StatusMatricula status)
    {
        // Arrange
        var matricula = MatriculaBuilder.Nova()
            .ComStatus(status)
            .Build();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            matricula.Concluir());

        exception.Message.Should().Contain("Apenas matrículas ativas podem ser concluídas");
    }

    [Fact]
    public void Concluir_NaoDeveAlterarDataCancelamento()
    {
        // Arrange
        var matricula = MatriculaBuilder.Nova().Build();

        // Act
        matricula.Concluir();

        // Assert
        matricula.DataCancelamento.Should().BeNull();
    }
}
