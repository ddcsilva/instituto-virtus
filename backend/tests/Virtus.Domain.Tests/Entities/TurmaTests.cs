namespace Virtus.Domain.Tests.Entities;

public class TurmaTests
{
    [Fact]
    public void Construtor_DeveInicializarCorretamente_QuandoParametrosValidos()
    {
        // Arrange
        var nome = "Turma de Violão Iniciante";
        var capacidade = 15;
        var tipo = TipoCurso.Violao;
        var professor = ProfessorBuilder.Novo().Build();

        // Act
        var turma = new Turma(nome, capacidade, tipo, professor);

        // Assert
        turma.Nome.Should().Be(nome);
        turma.Capacidade.Should().Be(capacidade);
        turma.Tipo.Should().Be(tipo);
        turma.Professor.Should().Be(professor);
        turma.ProfessorId.Should().Be(professor.Id);
        turma.Ativa.Should().BeTrue();
        turma.Matriculas.Should().BeEmpty();
        turma.Id.Should().Be(0);
        turma.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        turma.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Construtor_DeveLancarExcecao_QuandoNomeInvalido(string? nomeInvalido)
    {
        // Arrange
        var capacidade = 10;
        var tipo = TipoCurso.Violao;
        var professor = ProfessorBuilder.Novo().Build();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Turma(nomeInvalido!, capacidade, tipo, professor));

        exception.Message.Should().Contain("Nome da turma é obrigatório");
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoNomeMuitoCurto()
    {
        // Arrange
        var nome = "AB";
        var capacidade = 10;
        var tipo = TipoCurso.Violao;
        var professor = ProfessorBuilder.Novo().Build();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Turma(nome, capacidade, tipo, professor));

        exception.Message.Should().Contain("Nome da turma deve ter pelo menos 3 caracteres");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Construtor_DeveLancarExcecao_QuandoCapacidadeInvalida(int capacidadeInvalida)
    {
        // Arrange
        var nome = "Turma Teste";
        var tipo = TipoCurso.Violao;
        var professor = ProfessorBuilder.Novo().Build();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Turma(nome, capacidadeInvalida, tipo, professor));

        exception.Message.Should().Contain("Capacidade deve ser maior que zero");
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoCapacidadeMuitoAlta()
    {
        // Arrange
        var nome = "Turma Teste";
        var capacidade = 51;
        var tipo = TipoCurso.Violao;
        var professor = ProfessorBuilder.Novo().Build();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Turma(nome, capacidade, tipo, professor));

        exception.Message.Should().Contain("Capacidade não pode ser maior que 50");
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoProfessorNulo()
    {
        // Arrange
        var nome = "Turma Teste";
        var capacidade = 10;
        var tipo = TipoCurso.Violao;
        Professor professor = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Turma(nome, capacidade, tipo, professor));
    }

    [Theory]
    [InlineData(TipoCurso.Violao)]
    [InlineData(TipoCurso.Teclado)]
    [InlineData(TipoCurso.Canto)]
    [InlineData(TipoCurso.Teologia)]
    public void Construtor_DeveAceitarTodosOsTiposDeCurso(TipoCurso tipo)
    {
        // Arrange
        var nome = "Turma Teste";
        var capacidade = 10;
        var professor = ProfessorBuilder.Novo().Build();

        // Act
        var turma = new Turma(nome, capacidade, tipo, professor);

        // Assert
        turma.Tipo.Should().Be(tipo);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(50)]
    public void Construtor_DeveAceitarCapacidadesValidas(int capacidade)
    {
        // Arrange
        var nome = "Turma Teste";
        var tipo = TipoCurso.Violao;
        var professor = ProfessorBuilder.Novo().Build();

        // Act
        var turma = new Turma(nome, capacidade, tipo, professor);

        // Assert
        turma.Capacidade.Should().Be(capacidade);
    }

    [Fact]
    public void TemVagasDisponiveis_DeveRetornarTrue_QuandoTurmaVazia()
    {
        // Arrange
        var turma = TurmaBuilder.Nova()
            .ComCapacidade(10)
            .Build();

        // Act
        var resultado = turma.TemVagasDisponiveis();

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public void ObterQuantidadeVagasDisponiveis_DeveRetornarCapacidadeTotal_QuandoTurmaVazia()
    {
        // Arrange
        var capacidade = 15;
        var turma = TurmaBuilder.Nova()
            .ComCapacidade(capacidade)
            .Build();

        // Act
        var vagasDisponiveis = turma.ObterQuantidadeVagasDisponiveis();

        // Assert
        vagasDisponiveis.Should().Be(capacidade);
    }

    [Fact]
    public void Ativar_DeveDefinirComoAtiva_QuandoChamado()
    {
        // Arrange
        var turma = TurmaBuilder.Nova()
            .Inativa()
            .Build();

        var dataAnterior = turma.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        turma.Ativar();

        // Assert
        turma.Ativa.Should().BeTrue();
        turma.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Fact]
    public void Inativar_DeveDefinirComoInativa_QuandoChamado()
    {
        // Arrange
        var turma = TurmaBuilder.Nova().Build();
        var dataAnterior = turma.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        turma.Inativar();

        // Assert
        turma.Ativa.Should().BeFalse();
        turma.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Fact]
    public void Turma_DeveHerdarDeBaseEntity_QuandoCriada()
    {
        // Arrange & Act
        var turma = TurmaBuilder.Nova().Build();

        // Assert
        turma.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void Matriculas_DeveSerSomenteLeitura_QuandoAcessada()
    {
        // Arrange
        var turma = TurmaBuilder.Nova().Build();

        // Act
        var matriculas = turma.Matriculas;

        // Assert
        matriculas.Should().BeAssignableTo<IReadOnlyCollection<Matricula>>();
        matriculas.Should().NotBeNull();
        matriculas.Should().BeEmpty();
    }

    [Fact]
    public void Nome_DevePermitirExatamente3Caracteres()
    {
        // Arrange
        var nome = "ABC";
        var capacidade = 10;
        var tipo = TipoCurso.Violao;
        var professor = ProfessorBuilder.Novo().Build();

        // Act
        var turma = new Turma(nome, capacidade, tipo, professor);

        // Assert
        turma.Nome.Should().Be(nome);
    }

    [Fact]
    public void CapacidadeMaxima_DeveSerPermitida()
    {
        // Arrange
        var nome = "Turma Grande";
        var capacidade = 50; // Capacidade máxima
        var tipo = TipoCurso.Violao;
        var professor = ProfessorBuilder.Novo().Build();

        // Act
        var turma = new Turma(nome, capacidade, tipo, professor);

        // Assert
        turma.Capacidade.Should().Be(50);
    }

    [Fact]
    public void CapacidadeMinima_DeveSerPermitida()
    {
        // Arrange
        var nome = "Turma Individual";
        var capacidade = 1; // Capacidade mínima
        var tipo = TipoCurso.Violao;
        var professor = ProfessorBuilder.Novo().Build();

        // Act
        var turma = new Turma(nome, capacidade, tipo, professor);

        // Assert
        turma.Capacidade.Should().Be(1);
    }

    [Fact]
    public void Turma_DeveIniciarSempreAtiva_QuandoCriada()
    {
        // Arrange & Act
        var turma = TurmaBuilder.Nova().Build();

        // Assert
        turma.Ativa.Should().BeTrue();
    }

    [Fact]
    public void FluxoCompleto_DevePermitirAtivarEInativar()
    {
        // Arrange
        var turma = TurmaBuilder.Nova().Build();

        // Act & Assert
        turma.Ativa.Should().BeTrue();

        turma.Inativar();
        turma.Ativa.Should().BeFalse();

        turma.Ativar();
        turma.Ativa.Should().BeTrue();
    }

    [Fact]
    public void ProfessorId_DeveCorresponderAoProfessorAssociado()
    {
        // Arrange
        var professor = ProfessorBuilder.Novo().Build();
        var professorIdProperty = typeof(BaseEntity).GetProperty("Id");
        professorIdProperty?.SetValue(professor, 123);

        // Act
        var turma = new Turma("Turma Teste", 10, TipoCurso.Violao, professor);

        // Assert
        turma.ProfessorId.Should().Be(professor.Id);
        turma.Professor.Should().Be(professor);
    }

    [Theory]
    [InlineData("Turma de Violão para Iniciantes")]
    [InlineData("Curso Avançado de Teclado")]
    [InlineData("Aulas de Canto Lírico")]
    [InlineData("Teologia Bíblica Fundamentalista")]
    public void Nome_DeveAceitarNomesDescritivos(string nome)
    {
        // Arrange
        var capacidade = 15;
        var tipo = TipoCurso.Violao;
        var professor = ProfessorBuilder.Novo().Build();

        // Act
        var turma = new Turma(nome, capacidade, tipo, professor);

        // Assert
        turma.Nome.Should().Be(nome);
    }
}
