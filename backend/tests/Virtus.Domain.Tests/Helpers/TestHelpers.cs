using System.Reflection;
using FluentAssertions.Collections;
using FluentAssertions.Primitives;

namespace Virtus.Domain.Tests.Helpers;

/// <summary>
/// Classe de helpers para facilitar a criação e manipulação de testes
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Define o ID de uma entidade usando reflection (para simular comportamento do EF)
    /// </summary>
    /// <typeparam name="T">Tipo da entidade que herda de BaseEntity</typeparam>
    /// <param name="entidade">A entidade a ter o ID definido</param>
    /// <param name="id">O ID a ser definido</param>
    public static void DefinirId<T>(T entidade, int id) where T : BaseEntity
    {
        var propriedadeId = typeof(BaseEntity).GetProperty("Id",
            BindingFlags.Public | BindingFlags.Instance);

        if (propriedadeId?.SetMethod != null)
        {
            propriedadeId.SetValue(entidade, id);
        }
    }

    /// <summary>
    /// Define a data de criação de uma entidade usando reflection
    /// </summary>
    /// <typeparam name="T">Tipo da entidade que herda de BaseEntity</typeparam>
    /// <param name="entidade">A entidade a ter a data definida</param>
    /// <param name="dataCriacao">A data de criação a ser definida</param>
    public static void DefinirDataCriacao<T>(T entidade, DateTime dataCriacao) where T : BaseEntity
    {
        var propriedade = typeof(BaseEntity).GetProperty("DataCriacao",
            BindingFlags.Public | BindingFlags.Instance);

        if (propriedade?.SetMethod != null)
        {
            propriedade.SetValue(entidade, dataCriacao);
        }
    }

    /// <summary>
    /// Define a data de atualização de uma entidade usando reflection
    /// </summary>
    /// <typeparam name="T">Tipo da entidade que herda de BaseEntity</typeparam>
    /// <param name="entidade">A entidade a ter a data definida</param>
    /// <param name="dataAtualizacao">A data de atualização a ser definida</param>
    public static void DefinirDataAtualizacao<T>(T entidade, DateTime dataAtualizacao) where T : BaseEntity
    {
        var propriedade = typeof(BaseEntity).GetProperty("DataAtualizacao",
            BindingFlags.Public | BindingFlags.Instance);

        if (propriedade?.SetMethod != null)
        {
            propriedade.SetValue(entidade, dataAtualizacao);
        }
    }

    /// <summary>
    /// Simula o comportamento do Entity Framework ao carregar uma entidade do banco
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    /// <param name="entidade">A entidade a ser "carregada"</param>
    /// <param name="id">O ID da entidade</param>
    /// <param name="dataCriacao">Data de criação (opcional)</param>
    /// <param name="dataAtualizacao">Data de atualização (opcional)</param>
    public static void SimularCarregamentoDoBanco<T>(T entidade, int id,
        DateTime? dataCriacao = null, DateTime? dataAtualizacao = null) where T : BaseEntity
    {
        DefinirId(entidade, id);

        if (dataCriacao.HasValue)
        {
            DefinirDataCriacao(entidade, dataCriacao.Value);
        }

        if (dataAtualizacao.HasValue)
        {
            DefinirDataAtualizacao(entidade, dataAtualizacao.Value);
        }
    }

    /// <summary>
    /// Cria uma data no passado para testes
    /// </summary>
    /// <param name="diasAtras">Número de dias atrás</param>
    /// <returns>Data UTC no passado</returns>
    public static DateTime DataPassada(int diasAtras = 1)
    {
        return DateTime.UtcNow.AddDays(-diasAtras);
    }

    /// <summary>
    /// Cria uma data no futuro para testes
    /// </summary>
    /// <param name="diasFrente">Número de dias à frente</param>
    /// <returns>Data UTC no futuro</returns>
    public static DateTime DataFutura(int diasFrente = 1)
    {
        return DateTime.UtcNow.AddDays(diasFrente);
    }

    /// <summary>
    /// Verifica se um método é privado
    /// </summary>
    /// <param name="tipo">Tipo da classe</param>
    /// <param name="nomeMetodo">Nome do método</param>
    /// <returns>True se o método for privado</returns>
    public static bool MetodoEPrivado(Type tipo, string nomeMetodo)
    {
        var metodo = tipo.GetMethod(nomeMetodo,
            BindingFlags.NonPublic | BindingFlags.Instance);
        return metodo?.IsPrivate == true;
    }

    /// <summary>
    /// Verifica se um método é protegido
    /// </summary>
    /// <param name="tipo">Tipo da classe</param>
    /// <param name="nomeMetodo">Nome do método</param>
    /// <returns>True se o método for protegido</returns>
    public static bool MetodoEProtegido(Type tipo, string nomeMetodo)
    {
        var metodo = tipo.GetMethod(nomeMetodo,
            BindingFlags.NonPublic | BindingFlags.Instance);
        return metodo?.IsFamily == true;
    }

    /// <summary>
    /// Verifica se uma propriedade tem setter privado
    /// </summary>
    /// <param name="tipo">Tipo da classe</param>
    /// <param name="nomePropriedade">Nome da propriedade</param>
    /// <returns>True se o setter for privado</returns>
    public static bool PropriedadeTemSetterPrivado(Type tipo, string nomePropriedade)
    {
        var propriedade = tipo.GetProperty(nomePropriedade);
        return propriedade?.SetMethod?.IsPrivate == true;
    }

    /// <summary>
    /// Aguarda um tempo mínimo para garantir diferença em DateTime.UtcNow
    /// Exemplo de uso: await TestHelpers.AguardarTempo(10); // Substitui Thread.Sleep(10)
    /// </summary>
    /// <param name="milissegundos">Tempo em milissegundos (padrão: 10)</param>
    public static async Task AguardarTempo(int milissegundos = 10)
    {
        await Task.Delay(milissegundos);
    }

    /// <summary>
    /// Cria uma lista de entidades com IDs sequenciais para testes
    /// Exemplo de uso: TestHelpers.DefinirIdsSequenciais(listaAlunos, 1);
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    /// <param name="entidades">Lista de entidades</param>
    /// <param name="idInicial">ID inicial (padrão: 1)</param>
    public static void DefinirIdsSequenciais<T>(IEnumerable<T> entidades, int idInicial = 1)
        where T : BaseEntity
    {
        var id = idInicial;
        foreach (var entidade in entidades)
        {
            DefinirId(entidade, id++);
        }
    }

    /// <summary>
    /// Valida se uma entidade está em estado válido após criação
    /// Exemplo de uso: TestHelpers.ValidarEstadoInicialEntidade(pessoa);
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    /// <param name="entidade">Entidade a ser validada</param>
    /// <param name="toleranciaSegundos">Tolerância em segundos para validação de data</param>
    public static void ValidarEstadoInicialEntidade<T>(T entidade, int toleranciaSegundos = 5)
        where T : BaseEntity
    {
        entidade.Should().NotBeNull();
        entidade.Id.Should().Be(0);
        entidade.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(toleranciaSegundos));
        entidade.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(toleranciaSegundos));
        entidade.DataCriacao.Should().Be(entidade.DataAtualizacao);
    }
}

/// <summary>
/// Extensões para facilitar testes com FluentAssertions
/// </summary>
public static class TestExtensions
{
    /// <summary>
    /// Verifica se a data é UTC
    /// </summary>
    /// <param name="dateTime">Data a ser verificada</param>
    /// <returns>AndConstraint para encadeamento</returns>
    public static AndConstraint<DateTimeAssertions> BeUtc(this DateTimeAssertions dateTime)
    {
        dateTime.Subject.Should().NotBeNull();
        dateTime.Subject!.Value.Kind.Should().Be(DateTimeKind.Utc);
        return new AndConstraint<DateTimeAssertions>(dateTime);
    }

    /// <summary>
    /// Verifica se a coleção não é nula e está vazia
    /// </summary>
    /// <typeparam name="T">Tipo dos elementos da coleção</typeparam>
    /// <param name="collection">Coleção a ser verificada</param>
    /// <returns>AndConstraint para encadeamento</returns>
    public static AndConstraint<GenericCollectionAssertions<T>> NotBeNullAndBeEmpty<T>(
        this GenericCollectionAssertions<T> collection)
    {
        collection.NotBeNull();
        return collection.BeEmpty();
    }
}
