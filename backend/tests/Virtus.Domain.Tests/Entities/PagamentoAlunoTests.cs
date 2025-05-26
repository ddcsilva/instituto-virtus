using Virtus.Domain.Entidades;

namespace Virtus.Domain.Tests.Entities;

public class PagamentoAlunoTests
{
    [Fact]
    public void Construtor_DeveInicializarCorretamente_QuandoParametrosValidos()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo().Build();
        var aluno = AlunoBuilder.Novo().Build();
        var valor = 150.00m;

        // Act
        var pagamentoAluno = new PagamentoAluno(pagamento, aluno, valor);

        // Assert
        pagamentoAluno.Pagamento.Should().Be(pagamento);
        pagamentoAluno.PagamentoId.Should().Be(pagamento.Id);
        pagamentoAluno.Aluno.Should().Be(aluno);
        pagamentoAluno.AlunoId.Should().Be(aluno.Id);
        pagamentoAluno.Valor.Should().Be(valor);
        pagamentoAluno.Id.Should().Be(0);
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoPagamentoNulo()
    {
        // Arrange
        Pagamento pagamento = null!;
        var aluno = AlunoBuilder.Novo().Build();
        var valor = 100.00m;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new PagamentoAluno(pagamento, aluno, valor));
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoAlunoNulo()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo().Build();
        Aluno aluno = null!;
        var valor = 100.00m;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new PagamentoAluno(pagamento, aluno, valor));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50.00)]
    public void Construtor_DeveLancarExcecao_QuandoValorInvalido(decimal valorInvalido)
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo().Build();
        var aluno = AlunoBuilder.Novo().Build();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new PagamentoAluno(pagamento, aluno, valorInvalido));

        exception.Message.Should().Contain("Valor deve ser maior que zero");
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(1.00)]
    [InlineData(100.50)]
    [InlineData(999.99)]
    public void Construtor_DeveAceitarValoresValidos(decimal valorValido)
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo().Build();
        var aluno = AlunoBuilder.Novo().Build();

        // Act
        var pagamentoAluno = new PagamentoAluno(pagamento, aluno, valorValido);

        // Assert
        pagamentoAluno.Valor.Should().Be(valorValido);
    }

    [Fact]
    public void PagamentoId_DeveCorresponderAoPagamentoAssociado()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo().Build();
        var aluno = AlunoBuilder.Novo().Build();

        // Simular que o pagamento tem um ID (normalmente vem do banco)
        var pagamentoIdProperty = typeof(BaseEntity).GetProperty("Id");
        pagamentoIdProperty?.SetValue(pagamento, 123);

        // Act
        var pagamentoAluno = new PagamentoAluno(pagamento, aluno, 100.00m);

        // Assert
        pagamentoAluno.PagamentoId.Should().Be(pagamento.Id);
        pagamentoAluno.Pagamento.Should().Be(pagamento);
    }

    [Fact]
    public void AlunoId_DeveCorresponderAoAlunoAssociado()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo().Build();
        var aluno = AlunoBuilder.Novo().Build();

        // Simular que o aluno tem um ID (normalmente vem do banco)
        var alunoIdProperty = typeof(BaseEntity).GetProperty("Id");
        alunoIdProperty?.SetValue(aluno, 456);

        // Act
        var pagamentoAluno = new PagamentoAluno(pagamento, aluno, 100.00m);

        // Assert
        pagamentoAluno.AlunoId.Should().Be(aluno.Id);
        pagamentoAluno.Aluno.Should().Be(aluno);
    }

    [Fact]
    public void PagamentoAluno_NaoDeveHerdarDeBaseEntity()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo().Build();
        var aluno = AlunoBuilder.Novo().Build();

        // Act
        var pagamentoAluno = new PagamentoAluno(pagamento, aluno, 100.00m);

        // Assert
        pagamentoAluno.Should().NotBeAssignableTo<BaseEntity>();
        pagamentoAluno.Should().BeOfType<PagamentoAluno>();
    }

    [Fact]
    public void PagamentoAluno_DeveTerPropriedadeId()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo().Build();
        var aluno = AlunoBuilder.Novo().Build();

        // Act
        var pagamentoAluno = new PagamentoAluno(pagamento, aluno, 100.00m);

        // Assert
        pagamentoAluno.Id.Should().Be(0); // Valor padrão antes de salvar no banco
    }

    [Fact]
    public void Construtor_DeveDefinirTodasAsPropriedades_QuandoChamado()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo()
            .ComValor(500.00m)
            .ComObservacao("Pagamento teste")
            .Build();

        var aluno = AlunoBuilder.Novo().Build();
        var valor = 250.00m;

        // Act
        var pagamentoAluno = new PagamentoAluno(pagamento, aluno, valor);

        // Assert
        pagamentoAluno.Should().NotBeNull();
        pagamentoAluno.Pagamento.Should().NotBeNull();
        pagamentoAluno.Aluno.Should().NotBeNull();
        pagamentoAluno.Valor.Should().BePositive();
        pagamentoAluno.PagamentoId.Should().Be(pagamento.Id);
        pagamentoAluno.AlunoId.Should().Be(aluno.Id);
    }

    [Fact]
    public void PagamentoAluno_DevePermitirValorMuitoPequeno()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo().Build();
        var aluno = AlunoBuilder.Novo().Build();
        var valorMinimo = 0.01m;

        // Act
        var pagamentoAluno = new PagamentoAluno(pagamento, aluno, valorMinimo);

        // Assert
        pagamentoAluno.Valor.Should().Be(valorMinimo);
    }

    [Fact]
    public void PagamentoAluno_DevePermitirValorGrande()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo()
            .ComValor(10000.00m) // Valor máximo do pagamento
            .Build();
        var aluno = AlunoBuilder.Novo().Build();
        var valorGrande = 9999.99m;

        // Act
        var pagamentoAluno = new PagamentoAluno(pagamento, aluno, valorGrande);

        // Assert
        pagamentoAluno.Valor.Should().Be(valorGrande);
    }

    [Fact]
    public void Relacionamentos_DevemSerBidirecionais()
    {
        // Arrange
        var pagamento = PagamentoBuilder.Novo().Build();
        var aluno1 = AlunoBuilder.Novo().Build();
        var aluno2 = AlunoBuilder.Novo().Build();

        // Act
        var pagamentoAluno1 = new PagamentoAluno(pagamento, aluno1, 50.00m);
        var pagamentoAluno2 = new PagamentoAluno(pagamento, aluno2, 75.00m);

        // Assert
        pagamentoAluno1.Pagamento.Should().Be(pagamento);
        pagamentoAluno1.Aluno.Should().Be(aluno1);

        pagamentoAluno2.Pagamento.Should().Be(pagamento);
        pagamentoAluno2.Aluno.Should().Be(aluno2);
    }

    [Fact]
    public void PagamentoAluno_DeveSerInicializadoComConstrutorPrivado()
    {
        // Este teste verifica se existe um construtor privado parametrless
        // que é necessário para o Entity Framework

        // Arrange & Act
        var construtorPrivado = typeof(PagamentoAluno)
            .GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .FirstOrDefault(c => c.GetParameters().Length == 0);

        // Assert
        construtorPrivado.Should().NotBeNull("Entity deve ter construtor privado para EF");
    }
}
