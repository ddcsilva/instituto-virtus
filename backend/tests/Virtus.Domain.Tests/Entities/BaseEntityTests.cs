namespace Virtus.Domain.Tests.Entities;

// Classe concreta para testar BaseEntity (que é abstrata)
public class EntidadeTeste : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;

    private EntidadeTeste() { }

    public EntidadeTeste(string nome)
    {
        Nome = nome;
    }

    public void AtualizarNome(string novoNome)
    {
        Nome = novoNome;
        AtualizarData();
    }

    // Método para expor o método protegido para teste
    public void ChamarAtualizarData()
    {
        AtualizarData();
    }
}

public class BaseEntityTests
{
    [Fact]
    public void Construtor_DeveInicializarPropriedadesCorretamente_QuandoCriado()
    {
        // Arrange & Act
        var entidade = new EntidadeTeste("Teste");

        // Assert
        entidade.Id.Should().Be(0); // Valor padrão antes de persistir
        entidade.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        entidade.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        entidade.DataCriacao.Should().BeCloseTo(entidade.DataAtualizacao, TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public void DataCriacao_DeveSerUtc_QuandoCriada()
    {
        // Arrange & Act
        var entidade = new EntidadeTeste("Teste");

        // Assert
        entidade.DataCriacao.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void DataAtualizacao_DeveSerUtc_QuandoCriada()
    {
        // Arrange & Act
        var entidade = new EntidadeTeste("Teste");

        // Assert
        entidade.DataAtualizacao.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void AtualizarData_DeveAtualizarDataAtualizacao_QuandoChamado()
    {
        // Arrange
        var entidade = new EntidadeTeste("Teste");
        var dataOriginal = entidade.DataAtualizacao;

        Thread.Sleep(10); // Garantir diferença de tempo

        // Act
        entidade.ChamarAtualizarData();

        // Assert
        entidade.DataAtualizacao.Should().BeAfter(dataOriginal);
        entidade.DataCriacao.Should().BeCloseTo(dataOriginal, TimeSpan.FromMilliseconds(1)); // DataCriacao não deve mudar
    }

    [Fact]
    public void AtualizarData_NaoDeveAlterarDataCriacao_QuandoChamado()
    {
        // Arrange
        var entidade = new EntidadeTeste("Teste");
        var dataCriacaoOriginal = entidade.DataCriacao;

        Thread.Sleep(10);

        // Act
        entidade.ChamarAtualizarData();

        // Assert
        entidade.DataCriacao.Should().Be(dataCriacaoOriginal);
    }

    [Fact]
    public void AtualizarData_DeveSerUtc_QuandoChamado()
    {
        // Arrange
        var entidade = new EntidadeTeste("Teste");

        Thread.Sleep(10);

        // Act
        entidade.ChamarAtualizarData();

        // Assert
        entidade.DataAtualizacao.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Id_DeveIniciarComZero_QuandoCriado()
    {
        // Arrange & Act
        var entidade = new EntidadeTeste("Teste");

        // Assert
        entidade.Id.Should().Be(0);
    }

    [Fact]
    public void Id_DeveSerPrivateSet_ParaProtegerIntegridade()
    {
        // Arrange
        var entidade = new EntidadeTeste("Teste");
        var propriedadeId = typeof(BaseEntity).GetProperty("Id");

        // Act & Assert
        propriedadeId.Should().NotBeNull();
        propriedadeId!.CanRead.Should().BeTrue();
        propriedadeId.CanWrite.Should().BeTrue(); // Pode escrever, mas setter é private
        propriedadeId.SetMethod!.IsPrivate.Should().BeTrue();
    }

    [Fact]
    public void DataCriacao_DeveSerPrivateSet_ParaProtegerIntegridade()
    {
        // Arrange
        var propriedadeDataCriacao = typeof(BaseEntity).GetProperty("DataCriacao");

        // Act & Assert
        propriedadeDataCriacao.Should().NotBeNull();
        propriedadeDataCriacao!.CanRead.Should().BeTrue();
        propriedadeDataCriacao.CanWrite.Should().BeTrue();
        propriedadeDataCriacao.SetMethod!.IsPrivate.Should().BeTrue();
    }

    [Fact]
    public void DataAtualizacao_DeveSerPrivateSet_ParaProtegerIntegridade()
    {
        // Arrange
        var propriedadeDataAtualizacao = typeof(BaseEntity).GetProperty("DataAtualizacao");

        // Act & Assert
        propriedadeDataAtualizacao.Should().NotBeNull();
        propriedadeDataAtualizacao!.CanRead.Should().BeTrue();
        propriedadeDataAtualizacao.CanWrite.Should().BeTrue();
        propriedadeDataAtualizacao.SetMethod!.IsPrivate.Should().BeTrue();
    }

    [Fact]
    public void AtualizarData_DeveSerProtected_ParaPermitirHeranca()
    {
        // Arrange
        var metodoAtualizarData = typeof(BaseEntity)
            .GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == "AtualizarData");

        // Act & Assert
        metodoAtualizarData.Should().NotBeNull();
        metodoAtualizarData!.IsFamily.Should().BeTrue(); // IsFamily = protected
    }

    [Fact]
    public void BaseEntity_DeveSerAbstrata()
    {
        // Act & Assert
        typeof(BaseEntity).IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void ConstrutorProtegido_DeveExistir_ParaEF()
    {
        // Arrange
        var construtorProtegido = typeof(BaseEntity)
            .GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .FirstOrDefault(c => c.GetParameters().Length == 0);

        // Act & Assert
        construtorProtegido.Should().NotBeNull();
        construtorProtegido!.IsFamily.Should().BeTrue(); // protected
    }

    [Fact]
    public void MultiplasChamadasAtualizarData_DeveAtualizarCorretamente()
    {
        // Arrange
        var entidade = new EntidadeTeste("Teste");
        var dataOriginal = entidade.DataAtualizacao;

        // Act & Assert - Primeira atualização
        Thread.Sleep(10);
        entidade.ChamarAtualizarData();
        var primeiraAtualizacao = entidade.DataAtualizacao;
        primeiraAtualizacao.Should().BeAfter(dataOriginal);

        // Act & Assert - Segunda atualização
        Thread.Sleep(10);
        entidade.ChamarAtualizarData();
        var segundaAtualizacao = entidade.DataAtualizacao;
        segundaAtualizacao.Should().BeAfter(primeiraAtualizacao);

        // Verificar que DataCriacao permanece inalterada
        entidade.DataCriacao.Should().BeCloseTo(dataOriginal, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public void EntidadeFilha_DeveHerdarComportamentoCorreto()
    {
        // Arrange & Act
        var entidade = new EntidadeTeste("Teste Inicial");
        var dataOriginal = entidade.DataAtualizacao;

        Thread.Sleep(10);
        entidade.AtualizarNome("Teste Atualizado");

        // Assert
        entidade.Nome.Should().Be("Teste Atualizado");
        entidade.DataAtualizacao.Should().BeAfter(dataOriginal);
        entidade.DataCriacao.Should().BeCloseTo(dataOriginal, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public void TodasAsEntidades_DevemHerdarDeBaseEntity()
    {
        // Act & Assert
        typeof(Pessoa).Should().BeAssignableTo<BaseEntity>();
        typeof(Aluno).Should().BeAssignableTo<BaseEntity>();
        typeof(Professor).Should().BeAssignableTo<BaseEntity>();
        typeof(Turma).Should().BeAssignableTo<BaseEntity>();
        typeof(Matricula).Should().BeAssignableTo<BaseEntity>();
        typeof(Pagamento).Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void DataCriacao_DataAtualizacao_DevemSerIguaisNaCriacao()
    {
        // Arrange & Act
        var entidade = new EntidadeTeste("Teste");

        // Assert
        entidade.DataCriacao.Should().BeCloseTo(entidade.DataAtualizacao, TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public void DataCriacao_NaoDeveAlterarAposMultiplasAtualizacoes()
    {
        // Arrange
        var entidade = new EntidadeTeste("Teste");
        var dataCriacaoOriginal = entidade.DataCriacao;

        // Act - Múltiplas atualizações
        for (int i = 0; i < 5; i++)
        {
            Thread.Sleep(5);
            entidade.ChamarAtualizarData();
        }

        // Assert
        entidade.DataCriacao.Should().Be(dataCriacaoOriginal);
        entidade.DataAtualizacao.Should().BeAfter(dataCriacaoOriginal);
    }

    [Fact]
    public void Propriedades_DevemTerNomesCorretos()
    {
        // Arrange
        var tipoBaseEntity = typeof(BaseEntity);

        // Act & Assert
        tipoBaseEntity.GetProperty("Id").Should().NotBeNull();
        tipoBaseEntity.GetProperty("DataCriacao").Should().NotBeNull();
        tipoBaseEntity.GetProperty("DataAtualizacao").Should().NotBeNull();
    }

    [Fact]
    public void Propriedades_DevemTerTiposCorretos()
    {
        // Arrange
        var tipoBaseEntity = typeof(BaseEntity);

        // Act & Assert
        tipoBaseEntity.GetProperty("Id")!.PropertyType.Should().Be<int>();
        tipoBaseEntity.GetProperty("DataCriacao")!.PropertyType.Should().Be<DateTime>();
        tipoBaseEntity.GetProperty("DataAtualizacao")!.PropertyType.Should().Be<DateTime>();
    }
}
