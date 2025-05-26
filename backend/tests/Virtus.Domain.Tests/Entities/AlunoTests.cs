namespace Virtus.Domain.Tests.Entities;

public class AlunoTests
{
    [Fact]
    public void Construtor_DeveInicializarCorretamente_QuandoParametrosValidos()
    {
        // Arrange
        var pessoa = PessoaBuilder.Novo().Build();
        var responsavel = PessoaBuilder.Novo()
            .ComTipo(TipoPessoa.Responsavel)
            .Build();

        // Act
        var aluno = new Aluno(pessoa, responsavel);

        // Assert
        aluno.Pessoa.Should().Be(pessoa);
        aluno.PessoaId.Should().Be(pessoa.Id);
        aluno.Responsavel.Should().Be(responsavel);
        aluno.ResponsavelId.Should().Be(responsavel.Id);
        aluno.Status.Should().Be(StatusAluno.Ativo);
        aluno.Matriculas.Should().BeEmpty();
        aluno.Id.Should().Be(0);
        aluno.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        aluno.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Construtor_DeveInicializarSemResponsavel_QuandoResponsavelNulo()
    {
        // Arrange
        var pessoa = PessoaBuilder.Novo().Build();

        // Act
        var aluno = new Aluno(pessoa, null);

        // Assert
        aluno.Pessoa.Should().Be(pessoa);
        aluno.PessoaId.Should().Be(pessoa.Id);
        aluno.Responsavel.Should().BeNull();
        aluno.ResponsavelId.Should().BeNull();
        aluno.Status.Should().Be(StatusAluno.Ativo);
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoPessoaNula()
    {
        // Arrange
        Pessoa pessoa = null!;
        var responsavel = PessoaBuilder.Novo().Build();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Aluno(pessoa, responsavel));
    }

    [Fact]
    public void PodeMatricular_DeveRetornarTrue_QuandoAlunoAtivoETurmaComVagas()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo()
            .ComStatus(StatusAluno.Ativo)
            .Build();

        var turma = TurmaBuilder.Nova()
            .ComCapacidade(10)
            .Build();

        // Act
        var resultado = aluno.PodeMatricular(turma);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void PodeMatricular_DeveRetornarFalse_QuandoAlunoInativo()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo()
            .ComStatus(StatusAluno.Inativo)
            .Build();

        var turma = TurmaBuilder.Nova()
            .ComCapacidade(10)
            .Build();

        // Act
        var resultado = aluno.PodeMatricular(turma);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void PodeMatricular_DeveRetornarFalse_QuandoAlunoDesistente()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo()
            .ComStatus(StatusAluno.Desistente)
            .Build();

        var turma = TurmaBuilder.Nova()
            .ComCapacidade(10)
            .Build();

        // Act
        var resultado = aluno.PodeMatricular(turma);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void PodeMatricular_DeveRetornarFalse_QuandoAlunoNaListaDeEspera()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo()
            .ComStatus(StatusAluno.ListaEspera)
            .Build();

        var turma = TurmaBuilder.Nova()
            .ComCapacidade(10)
            .Build();

        // Act
        var resultado = aluno.PodeMatricular(turma);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void PodeMatricular_DeveLancarExcecao_QuandoTurmaNula()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo().Build();
        Turma turma = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => aluno.PodeMatricular(turma));
    }

    [Fact]
    public void Inativar_DeveAlterarStatusParaInativo_QuandoChamado()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo()
            .ComStatus(StatusAluno.Ativo)
            .Build();

        var dataAnterior = aluno.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        aluno.Inativar();

        // Assert
        aluno.Status.Should().Be(StatusAluno.Inativo);
        aluno.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Fact]
    public void Reativar_DeveAlterarStatusParaAtivo_QuandoChamado()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo()
            .ComStatus(StatusAluno.Inativo)
            .Build();

        var dataAnterior = aluno.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        aluno.Reativar();

        // Assert
        aluno.Status.Should().Be(StatusAluno.Ativo);
        aluno.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Fact]
    public void Desistir_DeveAlterarStatusParaDesistente_QuandoChamado()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo()
            .ComStatus(StatusAluno.Ativo)
            .Build();

        var dataAnterior = aluno.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        aluno.Desistir();

        // Assert
        aluno.Status.Should().Be(StatusAluno.Desistente);
        aluno.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Fact]
    public void AdicionarNaListaDeEspera_DeveAlterarStatusParaListaEspera_QuandoChamado()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo()
            .ComStatus(StatusAluno.Ativo)
            .Build();

        var dataAnterior = aluno.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        aluno.AdicionarNaListaDeEspera();

        // Assert
        aluno.Status.Should().Be(StatusAluno.ListaEspera);
        aluno.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Theory]
    [InlineData(StatusAluno.Ativo)]
    [InlineData(StatusAluno.Inativo)]
    [InlineData(StatusAluno.ListaEspera)]
    [InlineData(StatusAluno.Desistente)]
    public void StatusAluno_DeveAceitarTodosOsValores_QuandoValidos(StatusAluno status)
    {
        // Arrange
        var aluno = AlunoBuilder.Novo().Build();

        // Act & Assert - métodos específicos para cada status
        switch (status)
        {
            case StatusAluno.Ativo:
                aluno.Reativar();
                break;
            case StatusAluno.Inativo:
                aluno.Inativar();
                break;
            case StatusAluno.ListaEspera:
                aluno.AdicionarNaListaDeEspera();
                break;
            case StatusAluno.Desistente:
                aluno.Desistir();
                break;
        }

        aluno.Status.Should().Be(status);
    }

    [Fact]
    public void Aluno_DeveHerdarDeBaseEntity_QuandoCriado()
    {
        // Arrange & Act
        var aluno = AlunoBuilder.Novo().Build();

        // Assert
        aluno.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void Matriculas_DeveSerSomenteLeitura_QuandoAcessada()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo().Build();

        // Act
        var matriculas = aluno.Matriculas;

        // Assert
        matriculas.Should().BeAssignableTo<IReadOnlyCollection<Matricula>>();
        matriculas.Should().NotBeNull();
        matriculas.Should().BeEmpty();
    }

    [Fact]
    public void Construtor_DevePermitirResponsavelComQualquerTipoPessoa()
    {
        // Arrange
        var pessoa = PessoaBuilder.Novo().Build();
        var responsavelAluno = PessoaBuilder.Novo().ComTipo(TipoPessoa.Aluno).Build();
        var responsavelProfessor = PessoaBuilder.Novo().ComTipo(TipoPessoa.Professor).Build();
        var responsavelAdmin = PessoaBuilder.Novo().ComTipo(TipoPessoa.Administrador).Build();

        // Act & Assert
        var aluno1 = new Aluno(pessoa, responsavelAluno);
        var aluno2 = new Aluno(pessoa, responsavelProfessor);
        var aluno3 = new Aluno(pessoa, responsavelAdmin);

        aluno1.Responsavel.Should().Be(responsavelAluno);
        aluno2.Responsavel.Should().Be(responsavelProfessor);
        aluno3.Responsavel.Should().Be(responsavelAdmin);
    }

    [Fact]
    public void PodeMatricular_DeveFuncionar_QuandoTurmaTemUmaVaga()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo().Build();
        var turma = TurmaBuilder.Nova().ComCapacidade(1).Build();

        // Act
        var resultado = aluno.PodeMatricular(turma);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void FluxoCompleto_DevePermitirMudancasDeStatus()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo().Build();

        // Act & Assert - Fluxo: Ativo -> Inativo -> Ativo -> Lista Espera -> Ativo -> Desistente
        aluno.Status.Should().Be(StatusAluno.Ativo);

        aluno.Inativar();
        aluno.Status.Should().Be(StatusAluno.Inativo);

        aluno.Reativar();
        aluno.Status.Should().Be(StatusAluno.Ativo);

        aluno.AdicionarNaListaDeEspera();
        aluno.Status.Should().Be(StatusAluno.ListaEspera);

        aluno.Reativar();
        aluno.Status.Should().Be(StatusAluno.Ativo);

        aluno.Desistir();
        aluno.Status.Should().Be(StatusAluno.Desistente);
    }

    [Fact]
    public void AtualizarResponsavel_DeveDefinirNovoResponsavel_QuandoResponsavelValido()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo().Build();
        var novoResponsavel = PessoaBuilder.Novo()
            .ComTipo(TipoPessoa.Responsavel)
            .Build();

        var dataAnterior = aluno.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        aluno.AtualizarResponsavel(novoResponsavel);

        // Assert
        aluno.Responsavel.Should().Be(novoResponsavel);
        aluno.ResponsavelId.Should().Be(novoResponsavel.Id);
        aluno.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Fact]
    public void AtualizarResponsavel_DeveRemoverResponsavel_QuandoResponsavelNulo()
    {
        // Arrange
        var responsavelInicial = PessoaBuilder.Novo()
            .ComTipo(TipoPessoa.Responsavel)
            .Build();

        var aluno = AlunoBuilder.Novo()
            .ComResponsavel(responsavelInicial)
            .Build();

        var dataAnterior = aluno.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        aluno.AtualizarResponsavel(null);

        // Assert
        aluno.Responsavel.Should().BeNull();
        aluno.ResponsavelId.Should().BeNull();
        aluno.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

        [Fact]
    public void AtualizarResponsavel_DeveSubstituirResponsavel_QuandoJaExisteResponsavel()
    {
        // Arrange
        var responsavelInicial = PessoaBuilder.Novo()
            .ComTipo(TipoPessoa.Responsavel)
            .Build();

        var novoResponsavel = PessoaBuilder.Novo()
            .ComTipo(TipoPessoa.Responsavel)
            .Build();

        // Simular IDs diferentes para os responsáveis (como se fossem persistidos)
        SetEntityId(responsavelInicial, 1);
        SetEntityId(novoResponsavel, 2);

        var aluno = AlunoBuilder.Novo()
            .ComResponsavel(responsavelInicial)
            .Build();

        var dataAnterior = aluno.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        aluno.AtualizarResponsavel(novoResponsavel);

        // Assert
        aluno.Responsavel.Should().Be(novoResponsavel);
        aluno.ResponsavelId.Should().Be(novoResponsavel.Id);
        aluno.Responsavel.Should().NotBe(responsavelInicial);
        aluno.ResponsavelId.Should().NotBe(responsavelInicial.Id);
        aluno.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    private static void SetEntityId(BaseEntity entity, int id)
    {
        var idProperty = typeof(BaseEntity).GetProperty("Id");
        idProperty?.SetValue(entity, id);
    }

    [Fact]
    public void AtualizarResponsavel_DeveAtualizarDataAtualizacao_QuandoChamado()
    {
        // Arrange
        var aluno = AlunoBuilder.Novo().Build();
        var responsavel = PessoaBuilder.Novo()
            .ComTipo(TipoPessoa.Responsavel)
            .Build();

        var dataAnterior = aluno.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        aluno.AtualizarResponsavel(responsavel);

        // Assert
        aluno.DataAtualizacao.Should().BeAfter(dataAnterior);
    }
}
