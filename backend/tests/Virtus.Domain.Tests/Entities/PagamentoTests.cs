namespace Virtus.Domain.Tests.Entities;

public class PagamentoTests
{
    [Theory]
    [InlineData(100.00)]
    [InlineData(1.00)]
    [InlineData(500.50)]
    [InlineData(9999.99)]
    public void Construtor_DeveInicializarCorretamente_QuandoParametrosValidos(decimal valor)
    {
        // Arrange
        var dataPagamento = DateTime.UtcNow.AddDays(-1);
        var pagador = PessoaBuilder.Novo().Build();
        var observacao = "Pagamento de mensalidade";

        // Act
        var pagamento = new Pagamento(valor, dataPagamento, pagador, observacao);

        // Assert
        pagamento.Valor.Should().Be(valor);
        pagamento.DataPagamento.Should().Be(dataPagamento);
        pagamento.Pagador.Should().Be(pagador);
        pagamento.PagadorId.Should().Be(pagador.Id);
        pagamento.Observacao.Should().Be(observacao);
        pagamento.PagamentoAlunos.Should().BeEmpty();
        pagamento.Id.Should().Be(0);
        pagamento.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        pagamento.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Construtor_DeveInicializarSemObservacao_QuandoObservacaoNula()
    {
        // Arrange
        var valor = 100.00m;
        var dataPagamento = DateTime.UtcNow;
        var pagador = PessoaBuilder.Novo().Build();

        // Act
        var pagamento = new Pagamento(valor, dataPagamento, pagador, null);

        // Assert
        pagamento.Observacao.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Construtor_DeveLancarExcecao_QuandoValorInvalido(decimal valorInvalido)
    {
        // Arrange
        var dataPagamento = DateTime.UtcNow;
        var pagador = PessoaBuilder.Novo().Build();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Pagamento(valorInvalido, dataPagamento, pagador));

        exception.Message.Should().Contain("Valor do pagamento deve ser maior que zero");
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoValorMuitoAlto()
    {
        // Arrange
        var valor = 10000.01m; // Maior que 10.000
        var dataPagamento = DateTime.UtcNow;
        var pagador = PessoaBuilder.Novo().Build();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Pagamento(valor, dataPagamento, pagador));

        exception.Message.Should().Contain("Valor do pagamento não pode ser maior que R$ 10.000");
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoPagadorNulo()
    {
        // Arrange
        var valor = 100.00m;
        var dataPagamento = DateTime.UtcNow;
        Pessoa pagador = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Pagamento(valor, dataPagamento, pagador));
    }

    [Fact]
    public void AdicionarAluno_DeveAdicionarCorretamente_QuandoParametrosValidos()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo()
            .ComValor(200.00m)
            .Build();

        var aluno = AlunoBuilder.Novo().Build();
        var valorAluno = 100.00m;

        // Act
        pagamento.AdicionarAluno(aluno, valorAluno);

        // Assert
        pagamento.PagamentoAlunos.Should().HaveCount(1);
        var pagamentoAluno = pagamento.PagamentoAlunos.First();
        pagamentoAluno.Aluno.Should().Be(aluno);
        pagamentoAluno.Valor.Should().Be(valorAluno);
    }

    [Fact]
    public void AdicionarAluno_DeveLancarExcecao_QuandoAlunoNulo()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo().Build();
        Aluno aluno = null!;
        var valor = 50.00m;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            pagamento.AdicionarAluno(aluno, valor));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50.00)]
    public void AdicionarAluno_DeveLancarExcecao_QuandoValorInvalido(decimal valorInvalido)
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo().Build();
        var aluno = AlunoBuilder.Novo().Build();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            pagamento.AdicionarAluno(aluno, valorInvalido));

        exception.Message.Should().Contain("Valor por aluno deve ser maior que zero");
    }

    [Fact]
    public void AdicionarAluno_DeveLancarExcecao_QuandoAlunoJaIncluido()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo()
            .ComValor(200.00m)
            .Build();

        var aluno = AlunoBuilder.Novo().Build();
        pagamento.AdicionarAluno(aluno, 50.00m);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            pagamento.AdicionarAluno(aluno, 50.00m));

        exception.Message.Should().Contain("Aluno já está incluído neste pagamento");
    }

    [Fact]
    public void AdicionarAluno_DeveLancarExcecao_QuandoTotalExcedeValorPagamento()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo()
            .ComValor(100.00m)
            .Build();

        var aluno1 = AlunoBuilder.Novo().Build();
        var aluno2 = AlunoBuilder.Novo().Build();

        // Definir IDs únicos para os alunos
        TestHelpers.DefinirId(aluno1, 1);
        TestHelpers.DefinirId(aluno2, 2);

        pagamento.AdicionarAluno(aluno1, 60.00m);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            pagamento.AdicionarAluno(aluno2, 50.00m)); // 60 + 50 = 110 > 100

        exception.Message.Should().Contain("Total distribuído excede o valor do pagamento");
    }

    [Fact]
    public void ObterTotalDistribuido_DeveRetornarZero_QuandoNenhumAlunoAdicionado()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo().Build();

        // Act
        var total = pagamento.ObterTotalDistribuido();

        // Assert
        total.Should().Be(0);
    }

    [Fact]
    public void ObterTotalDistribuido_DeveRetornarSomaCorreta_QuandoVariosAlunos()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo()
            .ComValor(300.00m)
            .Build();

        var aluno1 = AlunoBuilder.Novo().Build();
        var aluno2 = AlunoBuilder.Novo().Build();
        var aluno3 = AlunoBuilder.Novo().Build();

        // Definir IDs únicos para os alunos
        TestHelpers.DefinirId(aluno1, 1);
        TestHelpers.DefinirId(aluno2, 2);
        TestHelpers.DefinirId(aluno3, 3);

        pagamento.AdicionarAluno(aluno1, 100.00m);
        pagamento.AdicionarAluno(aluno2, 75.50m);
        pagamento.AdicionarAluno(aluno3, 50.25m);

        // Act
        var total = pagamento.ObterTotalDistribuido();

        // Assert
        total.Should().Be(225.75m);
    }

    [Fact]
    public void EstaBalanceado_DeveRetornarTrue_QuandoTotalDistribuidoIgualValorPagamento()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo()
            .ComValor(150.00m)
            .Build();

        var aluno1 = AlunoBuilder.Novo().Build();
        var aluno2 = AlunoBuilder.Novo().Build();

        // Definir IDs únicos para os alunos
        TestHelpers.DefinirId(aluno1, 1);
        TestHelpers.DefinirId(aluno2, 2);

        pagamento.AdicionarAluno(aluno1, 100.00m);
        pagamento.AdicionarAluno(aluno2, 50.00m);

        // Act
        var balanceado = pagamento.EstaBalanceado();

        // Assert
        balanceado.Should().BeTrue();
    }

    [Fact]
    public void EstaBalanceado_DeveRetornarFalse_QuandoTotalDistribuidoDiferenteValorPagamento()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo()
            .ComValor(200.00m)
            .Build();

        var aluno = AlunoBuilder.Novo().Build();
        pagamento.AdicionarAluno(aluno, 100.00m);

        // Act
        var balanceado = pagamento.EstaBalanceado();

        // Assert
        balanceado.Should().BeFalse();
    }

    [Fact]
    public void EstaBalanceado_DeveRetornarTrue_QuandoNenhumAlunoEValorZero()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo()
            .ComValor(1.00m) // Valor mínimo válido
            .Build();

        // Act
        var balanceado = pagamento.EstaBalanceado();

        // Assert
        balanceado.Should().BeFalse(); // 1.00 != 0
    }

    [Fact]
    public void Pagamento_DeveHerdarDeBaseEntity_QuandoCriado()
    {
        // Arrange & Act
        var pagamento = PagamentoBuilder.Novo().Build();

        // Assert
        pagamento.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void PagamentoAlunos_DeveSerSomenteLeitura_QuandoAcessada()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo().Build();

        // Act
        var pagamentoAlunos = pagamento.PagamentoAlunos;

        // Assert
        pagamentoAlunos.Should().BeAssignableTo<IReadOnlyCollection<Virtus.Domain.Entidades.PagamentoAluno>>();
        pagamentoAlunos.Should().NotBeNull();
        pagamentoAlunos.Should().BeEmpty();
    }

    [Fact]
    public void ValorMaximo_DeveSerPermitido()
    {
        // Arrange
        var valor = 10000.00m; // Valor máximo permitido
        var dataPagamento = DateTime.UtcNow;
        var pagador = PessoaBuilder.Novo().Build();

        // Act
        var pagamento = new Pagamento(valor, dataPagamento, pagador);

        // Assert
        pagamento.Valor.Should().Be(valor);
    }

    [Fact]
    public void ValorMinimo_DeveSerPermitido()
    {
        // Arrange
        var valor = 0.01m; // Valor mínimo válido
        var dataPagamento = DateTime.UtcNow;
        var pagador = PessoaBuilder.Novo().Build();

        // Act
        var pagamento = new Pagamento(valor, dataPagamento, pagador);

        // Assert
        pagamento.Valor.Should().Be(valor);
    }

    [Fact]
    public void AdicionarAluno_DeveAtualizarDataAtualizacao()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo().Build();
        var aluno = AlunoBuilder.Novo().Build();
        var dataAnterior = pagamento.DataAtualizacao;

        Thread.Sleep(10);

        // Act
        pagamento.AdicionarAluno(aluno, 50.00m);

        // Assert
        pagamento.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Fact]
    public void PagadorId_DeveCorresponderAoPagadorAssociado()
    {
        // Arrange
        var pagador = PessoaBuilder.Novo().Build();

        // Simular que o pagador tem um ID (normalmente vem do banco)
        var pagadorIdProperty = typeof(BaseEntity).GetProperty("Id");
        pagadorIdProperty?.SetValue(pagador, 999);

        // Act
        var pagamento = new Pagamento(100.00m, DateTime.UtcNow, pagador);

        // Assert
        pagamento.PagadorId.Should().Be(pagador.Id);
        pagamento.Pagador.Should().Be(pagador);
    }

    [Fact]
    public void FluxoCompleto_DevePermitirAdicionarMultiplosAlunos()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo()
            .ComValor(300.00m)
            .Build();

        var aluno1 = AlunoBuilder.Novo().Build();
        var aluno2 = AlunoBuilder.Novo().Build();
        var aluno3 = AlunoBuilder.Novo().Build();

        // Definir IDs únicos para os alunos
        TestHelpers.DefinirId(aluno1, 1);
        TestHelpers.DefinirId(aluno2, 2);
        TestHelpers.DefinirId(aluno3, 3);

        // Act
        pagamento.AdicionarAluno(aluno1, 100.00m);
        pagamento.AdicionarAluno(aluno2, 100.00m);
        pagamento.AdicionarAluno(aluno3, 100.00m);

        // Assert
        pagamento.PagamentoAlunos.Should().HaveCount(3);
        pagamento.ObterTotalDistribuido().Should().Be(300.00m);
        pagamento.EstaBalanceado().Should().BeTrue();
    }
}
