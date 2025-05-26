namespace Virtus.Domain.Tests.Entities;

public class ProfessorTests
{
    [Fact]
    public void Construtor_DeveInicializarCorretamente_QuandoParametrosValidos()
    {
        // Arrange
        var pessoa = PessoaBuilder.Novo()
            .ComTipo(TipoPessoa.Professor)
            .Build();

        // Act
        var professor = new Professor(pessoa);

        // Assert
        professor.Pessoa.Should().Be(pessoa);
        professor.PessoaId.Should().Be(pessoa.Id);
        professor.Ativo.Should().BeTrue();
        professor.Turmas.Should().BeEmpty();
        professor.Id.Should().Be(0);
        professor.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        professor.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoPessoaNula()
    {
        // Arrange
        Pessoa pessoa = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Professor(pessoa));
    }

    [Theory]
    [InlineData(TipoPessoa.Aluno)]
    [InlineData(TipoPessoa.Responsavel)]
    [InlineData(TipoPessoa.Administrador)]
    public void Construtor_DevePermitirQualquerTipoPessoa(TipoPessoa tipo)
    {
        // Arrange
        var pessoa = PessoaBuilder.Novo()
            .ComTipo(tipo)
            .Build();

        // Act
        var professor = new Professor(pessoa);

        // Assert
        professor.Pessoa.Tipo.Should().Be(tipo);
        professor.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Ativar_DeveDefinirComoAtivo_QuandoChamado()
    {
        // Arrange
        var professor = ProfessorBuilder.Novo()
            .Inativo()
            .Build();

        var dataAnterior = professor.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        professor.Ativar();

        // Assert
        professor.Ativo.Should().BeTrue();
        professor.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Fact]
    public void Inativar_DeveDefinirComoInativo_QuandoChamado()
    {
        // Arrange
        var professor = ProfessorBuilder.Novo().Build(); // Ativo por padrão
        var dataAnterior = professor.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        professor.Inativar();

        // Assert
        professor.Ativo.Should().BeFalse();
        professor.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Fact]
    public void Professor_DeveHerdarDeBaseEntity_QuandoCriado()
    {
        // Arrange & Act
        var professor = ProfessorBuilder.Novo().Build();

        // Assert
        professor.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void Turmas_DeveSerSomenteLeitura_QuandoAcessada()
    {
        // Arrange
        var professor = ProfessorBuilder.Novo().Build();

        // Act
        var turmas = professor.Turmas;

        // Assert
        turmas.Should().BeAssignableTo<IReadOnlyCollection<Turma>>();
        turmas.Should().NotBeNull();
        turmas.Should().BeEmpty();
    }

    [Fact]
    public void PessoaId_DeveCorresponderAPessoaAssociada()
    {
        // Arrange
        var pessoa = PessoaBuilder.Novo().Build();

        // Simular que a pessoa tem um ID (normalmente vem do banco)
        var pessoaIdProperty = typeof(BaseEntity).GetProperty("Id");
        pessoaIdProperty?.SetValue(pessoa, 789);

        // Act
        var professor = new Professor(pessoa);

        // Assert
        professor.PessoaId.Should().Be(pessoa.Id);
        professor.Pessoa.Should().Be(pessoa);
    }

    [Fact]
    public void Professor_DeveIniciarSempreAtivo_QuandoCriado()
    {
        // Arrange
        var pessoa = PessoaBuilder.Novo().Build();

        // Act
        var professor = new Professor(pessoa);

        // Assert
        professor.Ativo.Should().BeTrue();
    }

    [Fact]
    public void FluxoCompleto_DevePermitirAtivarEInativar()
    {
        // Arrange
        var professor = ProfessorBuilder.Novo().Build();

        // Act & Assert - Fluxo: Ativo -> Inativo -> Ativo
        professor.Ativo.Should().BeTrue();

        professor.Inativar();
        professor.Ativo.Should().BeFalse();

        professor.Ativar();
        professor.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Ativar_NaoDeveAlterarEstado_QuandoJaAtivo()
    {
        // Arrange
        var professor = ProfessorBuilder.Novo().Build(); // Já ativo
        var dataAnterior = professor.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        professor.Ativar();

        // Assert
        professor.Ativo.Should().BeTrue();
        professor.DataAtualizacao.Should().BeAfter(dataAnterior); // Deve atualizar mesmo se já ativo
    }

    [Fact]
    public void Inativar_NaoDeveAlterarEstado_QuandoJaInativo()
    {
        // Arrange
        var professor = ProfessorBuilder.Novo()
            .Inativo()
            .Build();

        var dataAnterior = professor.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        professor.Inativar();

        // Assert
        professor.Ativo.Should().BeFalse();
        professor.DataAtualizacao.Should().BeAfter(dataAnterior); // Deve atualizar mesmo se já inativo
    }

    [Fact]
    public void Professor_DevePermitirAssociacaoComPessoaDeQualquerTipo()
    {
        // Arrange
        var pessoaAluno = PessoaBuilder.Novo().ComTipo(TipoPessoa.Aluno).Build();
        var pessoaResponsavel = PessoaBuilder.Novo().ComTipo(TipoPessoa.Responsavel).Build();
        var pessoaAdmin = PessoaBuilder.Novo().ComTipo(TipoPessoa.Administrador).Build();
        var pessoaProfessor = PessoaBuilder.Novo().ComTipo(TipoPessoa.Professor).Build();

        // Act & Assert
        var professor1 = new Professor(pessoaAluno);
        var professor2 = new Professor(pessoaResponsavel);
        var professor3 = new Professor(pessoaAdmin);
        var professor4 = new Professor(pessoaProfessor);

        professor1.Pessoa.Tipo.Should().Be(TipoPessoa.Aluno);
        professor2.Pessoa.Tipo.Should().Be(TipoPessoa.Responsavel);
        professor3.Pessoa.Tipo.Should().Be(TipoPessoa.Administrador);
        professor4.Pessoa.Tipo.Should().Be(TipoPessoa.Professor);
    }

    [Fact]
    public void Professor_DeveManterReferenciaBidirecional()
    {
        // Arrange
        var pessoa = PessoaBuilder.Novo()
            .ComNome("Maria Silva")
            .ComEmail("maria@professor.com")
            .ComTipo(TipoPessoa.Professor)
            .Build();

        // Act
        var professor = new Professor(pessoa);

        // Assert
        professor.Pessoa.Should().Be(pessoa);
        professor.Pessoa.Nome.Should().Be("Maria Silva");
        professor.Pessoa.Email.Endereco.Should().Be("maria@professor.com");
        professor.Pessoa.Tipo.Should().Be(TipoPessoa.Professor);
    }

    [Fact]
    public void MultiplasOperacoes_DeveAtualizarDataCorretamente()
    {
        // Arrange
        var professor = ProfessorBuilder.Novo().Build();
        var dataOriginal = professor.DataAtualizacao;

        // Act & Assert - Múltiplas operações
        Thread.Sleep(10);
        professor.Inativar();
        var dataAposInativar = professor.DataAtualizacao;
        dataAposInativar.Should().BeAfter(dataOriginal);

        Thread.Sleep(10);
        professor.Ativar();
        var dataAposAtivar = professor.DataAtualizacao;
        dataAposAtivar.Should().BeAfter(dataAposInativar);

        Thread.Sleep(10);
        professor.Inativar();
        var dataFinal = professor.DataAtualizacao;
        dataFinal.Should().BeAfter(dataAposAtivar);
    }

    [Fact]
    public void Professor_DeveSerCriadoComPropriedadesCorretas()
    {
        // Arrange
        var pessoa = PessoaBuilder.Novo()
            .ComNome("João Professor")
            .ComEmail("joao.professor@escola.com")
            .ComTelefone("(11) 99999-9999")
            .ComTipo(TipoPessoa.Professor)
            .Build();

        // Act
        var professor = new Professor(pessoa);

        // Assert
        professor.Should().NotBeNull();
        professor.Pessoa.Should().NotBeNull();
        professor.Pessoa.Nome.Should().Be("João Professor");
        professor.Pessoa.Email.Endereco.Should().Be("joao.professor@escola.com");
        professor.Pessoa.Telefone.Should().Be("(11) 99999-9999");
        professor.Ativo.Should().BeTrue();
        professor.Turmas.Should().BeEmpty();
    }
}
