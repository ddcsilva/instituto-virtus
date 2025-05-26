namespace Virtus.Domain.Tests.Entities;

public class PessoaTests
{
    [Fact]
    public void Construtor_DeveInicializarCorretamente_QuandoParametrosValidos()
    {
        // Arrange
        var nome = "João Silva";
        var email = Email.Criar("joao@email.com");
        var telefone = "(11) 99999-9999";
        var tipo = TipoPessoa.Aluno;

        // Act
        var pessoa = new Pessoa(nome, email, telefone, tipo);

        // Assert
        pessoa.Nome.Should().Be(nome);
        pessoa.Email.Should().Be(email);
        pessoa.Telefone.Should().Be(telefone);
        pessoa.Tipo.Should().Be(tipo);
        pessoa.Id.Should().Be(0);
        pessoa.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        pessoa.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Construtor_DeveLancarExcecao_QuandoNomeInvalido(string? nomeInvalido)
    {
        // Arrange
        var email = Email.Criar("joao@email.com");
        var telefone = "(11) 99999-9999";
        var tipo = TipoPessoa.Aluno;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Pessoa(nomeInvalido!, email, telefone, tipo));

        exception.Message.Should().Contain("Nome é obrigatório");
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoNomeMuitoCurto()
    {
        // Arrange
        var nome = "Jo"; // Apenas 2 caracteres
        var email = Email.Criar("joao@email.com");
        var telefone = "(11) 99999-9999";
        var tipo = TipoPessoa.Aluno;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Pessoa(nome, email, telefone, tipo));

        exception.Message.Should().Contain("Nome deve ter pelo menos 3 caracteres");
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoNomeMuitoLongo()
    {
        // Arrange
        var nome = new string('A', 101);
        var email = Email.Criar("joao@email.com");
        var telefone = "(11) 99999-9999";
        var tipo = TipoPessoa.Aluno;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Pessoa(nome, email, telefone, tipo));

        exception.Message.Should().Contain("Nome não pode ter mais de 100 caracteres");
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoEmailNulo()
    {
        // Arrange
        var nome = "João Silva";
        Email email = null!;
        var telefone = "(11) 99999-9999";
        var tipo = TipoPessoa.Aluno;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Pessoa(nome, email, telefone, tipo));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Construtor_DeveLancarExcecao_QuandoTelefoneInvalido(string? telefoneInvalido)
    {
        // Arrange
        var nome = "João Silva";
        var email = Email.Criar("joao@email.com");
        var tipo = TipoPessoa.Aluno;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Pessoa(nome, email, telefoneInvalido!, tipo));

        exception.Message.Should().Contain("Telefone é obrigatório");
    }

    [Fact]
    public void AtualizarDados_DeveAtualizarCorretamente_QuandoParametrosValidos()
    {
        // Arrange
        var pessoa = PessoaBuilder.Novo().Build();
        var dataAnterior = pessoa.DataAtualizacao;

        var novoNome = "Maria Santos";
        var novoEmail = Email.Criar("maria@email.com");
        var novoTelefone = "(21) 88888-8888";

        Thread.Sleep(10);

        // Act
        pessoa.AtualizarDados(novoNome, novoEmail, novoTelefone);

        // Assert
        pessoa.Nome.Should().Be(novoNome);
        pessoa.Email.Should().Be(novoEmail);
        pessoa.Telefone.Should().Be(novoTelefone);
        pessoa.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void AtualizarDados_DeveLancarExcecao_QuandoNomeInvalido(string? nomeInvalido)
    {
        // Arrange
        var pessoa = PessoaBuilder.Novo().Build();
        var email = Email.Criar("novo@email.com");
        var telefone = "(11) 99999-9999";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            pessoa.AtualizarDados(nomeInvalido!, email, telefone));

        exception.Message.Should().Contain("Nome é obrigatório");
    }

    [Fact]
    public void AtualizarDados_DeveLancarExcecao_QuandoEmailNulo()
    {
        // Arrange
        var pessoa = PessoaBuilder.Novo().Build();
        var nome = "Novo Nome";
        Email email = null!;
        var telefone = "(11) 99999-9999";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            pessoa.AtualizarDados(nome, email, telefone));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void AtualizarDados_DeveLancarExcecao_QuandoTelefoneInvalido(string? telefoneInvalido)
    {
        // Arrange
        var pessoa = PessoaBuilder.Novo().Build();
        var nome = "Novo Nome";
        var email = Email.Criar("novo@email.com");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            pessoa.AtualizarDados(nome, email, telefoneInvalido!));

        exception.Message.Should().Contain("Telefone é obrigatório");
    }

    [Fact]
    public void AlterarTipo_DeveAlterarCorretamente_QuandoTipoValido()
    {
        // Arrange
        var pessoa = PessoaBuilder.Novo()
            .ComTipo(TipoPessoa.Aluno)
            .Build();

        var dataAnterior = pessoa.DataAtualizacao;
        Thread.Sleep(10);

        // Act
        pessoa.AlterarTipo(TipoPessoa.Professor);

        // Assert
        pessoa.Tipo.Should().Be(TipoPessoa.Professor);
        pessoa.DataAtualizacao.Should().BeAfter(dataAnterior);
    }

    [Theory]
    [InlineData(TipoPessoa.Aluno)]
    [InlineData(TipoPessoa.Professor)]
    [InlineData(TipoPessoa.Responsavel)]
    [InlineData(TipoPessoa.Administrador)]
    public void AlterarTipo_DeveAceitarTodosOsTipos_QuandoTiposValidos(TipoPessoa tipo)
    {
        // Arrange
        var pessoa = PessoaBuilder.Novo().Build();

        // Act
        pessoa.AlterarTipo(tipo);

        // Assert
        pessoa.Tipo.Should().Be(tipo);
    }

    [Fact]
    public void Pessoa_DeveSerCriadaComPropriedadesBase_QuandoHerdaDeBaseEntity()
    {
        // Arrange & Act
        var pessoa = PessoaBuilder.Novo().Build();

        // Assert
        pessoa.Should().BeAssignableTo<BaseEntity>();
        pessoa.Id.Should().Be(0);
        pessoa.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        pessoa.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Pessoa_DevePermitirNomeComExatamente3Caracteres()
    {
        // Arrange
        var nome = "Ana";
        var email = Email.Criar("ana@email.com");
        var telefone = "(11) 99999-9999";
        var tipo = TipoPessoa.Aluno;

        // Act
        var pessoa = new Pessoa(nome, email, telefone, tipo);

        // Assert
        pessoa.Nome.Should().Be(nome);
    }

    [Fact]
    public void Pessoa_DevePermitirNomeComExatamente100Caracteres()
    {
        // Arrange
        var nome = new string('A', 100);
        var email = Email.Criar("teste@email.com");
        var telefone = "(11) 99999-9999";
        var tipo = TipoPessoa.Aluno;

        // Act
        var pessoa = new Pessoa(nome, email, telefone, tipo);

        // Assert
        pessoa.Nome.Should().Be(nome);
        pessoa.Nome.Length.Should().Be(100);
    }
}
