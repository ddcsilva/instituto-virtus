namespace Virtus.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("teste@exemplo.com")]
    [InlineData("usuario@dominio.com.br")]
    [InlineData("nome.sobrenome@empresa.org")]
    [InlineData("user123@test-domain.net")]
    [InlineData("a@b.co")]
    [InlineData("test_email@domain.com")]
    [InlineData("test-email@domain.com")]
    [InlineData("test.email+tag@domain.com")]
    public void Criar_DeveRetornarEmailValido_QuandoFormatoCorreto(string enderecoValido)
    {
        // Act
        var email = Email.Criar(enderecoValido);

        // Assert
        email.Should().NotBeNull();
        email.Endereco.Should().Be(enderecoValido.ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("email-invalido")]
    [InlineData("@dominio.com")]
    [InlineData("usuario@")]
    [InlineData("usuario@dominio")]
    [InlineData("usuario.dominio.com")]
    [InlineData("usuario@@dominio.com")]
    [InlineData("usuário@domínio.com")]
    public void Criar_DeveLancarExcecao_QuandoFormatoInvalido(string enderecoInvalido)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Email.Criar(enderecoInvalido));
        exception.Message.Should().Contain("formato inválido");
    }

    [Fact]
    public void Criar_DeveLancarExcecao_QuandoEnderecoNulo()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Email.Criar(null!));
        exception.Message.Should().Contain("não pode ser vazio");
    }

    [Fact]
    public void TentarCriar_DeveRetornarTrue_QuandoEmailValido()
    {
        // Arrange
        var enderecoValido = "teste@exemplo.com";

        // Act
        var resultado = Email.TentarCriar(enderecoValido, out var email);

        // Assert
        resultado.Should().BeTrue();
        email.Should().NotBeNull();
        email!.Endereco.Should().Be(enderecoValido);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("email-invalido")]
    [InlineData("@dominio.com")]
    public void TentarCriar_DeveRetornarFalse_QuandoEmailInvalido(string? enderecoInvalido)
    {
        // Act
        var resultado = Email.TentarCriar(enderecoInvalido!, out var email);

        // Assert
        resultado.Should().BeFalse();
        email.Should().BeNull();
    }

    [Fact]
    public void Email_DeveNormalizarParaMinusculo_QuandoCriado()
    {
        // Arrange
        var enderecoComMaiusculas = "TESTE@EXEMPLO.COM";

        // Act
        var email = Email.Criar(enderecoComMaiusculas);

        // Assert
        email.Endereco.Should().Be("teste@exemplo.com");
    }

    [Fact]
    public void Email_DeveRemoverEspacos_QuandoCriado()
    {
        // Arrange
        var enderecoComEspacos = "  teste@exemplo.com  ";

        // Act
        var email = Email.Criar(enderecoComEspacos);

        // Assert
        email.Endereco.Should().Be("teste@exemplo.com");
    }

    [Fact]
    public void Equals_DeveRetornarTrue_QuandoEmailsIguais()
    {
        // Arrange
        var endereco = "teste@exemplo.com";
        var email1 = Email.Criar(endereco);
        var email2 = Email.Criar(endereco);

        // Act & Assert
        email1.Equals(email2).Should().BeTrue();
        (email1 == email2).Should().BeTrue();
        (email1 != email2).Should().BeFalse();
    }

    [Fact]
    public void Equals_DeveRetornarFalse_QuandoEmailsDiferentes()
    {
        // Arrange
        var email1 = Email.Criar("teste1@exemplo.com");
        var email2 = Email.Criar("teste2@exemplo.com");

        // Act & Assert
        email1.Equals(email2).Should().BeFalse();
        (email1 == email2).Should().BeFalse();
        (email1 != email2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DeveRetornarFalse_QuandoCompararComNull()
    {
        // Arrange
        var email = Email.Criar("teste@exemplo.com");

        // Act & Assert
        email.Equals(null).Should().BeFalse();
        (email == null).Should().BeFalse();
        (email != null).Should().BeTrue();
    }

    [Fact]
    public void Equals_DeveRetornarTrue_QuandoCompararComMesmoObjeto()
    {
        // Arrange
        var endereco = "teste@exemplo.com";
        var email1 = Email.Criar(endereco);
        var email2 = Email.Criar(endereco);

        // Act & Assert
        email1.Equals(email1).Should().BeTrue();
        (email1 == email2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_DeveSerIgual_QuandoEmailsIguais()
    {
        // Arrange
        var endereco = "teste@exemplo.com";
        var email1 = Email.Criar(endereco);
        var email2 = Email.Criar(endereco);

        // Act & Assert
        email1.GetHashCode().Should().Be(email2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DeveSerDiferente_QuandoEmailsDiferentes()
    {
        // Arrange
        var email1 = Email.Criar("teste1@exemplo.com");
        var email2 = Email.Criar("teste2@exemplo.com");

        // Act & Assert
        email1.GetHashCode().Should().NotBe(email2.GetHashCode());
    }

    [Fact]
    public void ToString_DeveRetornarEndereco()
    {
        // Arrange
        var endereco = "teste@exemplo.com";
        var email = Email.Criar(endereco);

        // Act
        var resultado = email.ToString();

        // Assert
        resultado.Should().Be(endereco);
    }

    [Fact]
    public void ImplicitOperator_DeveConverterParaString()
    {
        // Arrange
        var endereco = "teste@exemplo.com";
        var email = Email.Criar(endereco);

        // Act
        string resultado = email;

        // Assert
        resultado.Should().Be(endereco);
    }

    [Fact]
    public void ImplicitOperator_DeveRetornarStringVazia_QuandoEmailNulo()
    {
        // Arrange
        Email email = null!;

        // Act
        string resultado = email;

        // Assert
        resultado.Should().Be(string.Empty);
    }

    [Fact]
    public void OperatorEquality_DeveCompararCorrente_QuandoAmbosNulos()
    {
        // Arrange
        Email email1 = null!;
        Email email2 = null!;

        // Act & Assert
        (email1 == email2).Should().BeTrue();
        (email1 != email2).Should().BeFalse();
    }

    [Fact]
    public void OperatorEquality_DeveCompararCorretamente_QuandoUmNulo()
    {
        // Arrange
        var email1 = Email.Criar("teste@exemplo.com");
        Email email2 = null!;

        // Act & Assert
        (email1 == email2).Should().BeFalse();
        (email2 == email1).Should().BeFalse();
        (email1 != email2).Should().BeTrue();
        (email2 != email1).Should().BeTrue();
    }

    [Theory]
    [InlineData("TESTE@EXEMPLO.COM", "teste@exemplo.com")]
    [InlineData("  Email@Domain.Com  ", "email@domain.com")]
    [InlineData("USER@COMPANY.ORG", "user@company.org")]
    public void Email_DeveNormalizarFormatoCorreto_QuandoDiferentesFormatos(string entrada, string esperado)
    {
        // Act
        var email = Email.Criar(entrada);

        // Assert
        email.Endereco.Should().Be(esperado);
    }
}
