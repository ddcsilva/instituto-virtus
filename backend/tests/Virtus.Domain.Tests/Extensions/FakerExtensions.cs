using Bogus;

namespace Virtus.Domain.Tests.Extensions;

public static class FakerExtensions
{
    private static readonly Faker _faker = new("pt_BR");

    // Dados pessoais
    public static string NomeCompleto() => _faker.Name.FullName();
    public static string PrimeiroNome() => _faker.Name.FirstName();
    public static string Email() => _faker.Internet.Email().ToLowerInvariant();
    public static string Telefone() => _faker.Phone.PhoneNumber("(##) #####-####");

    // Valores monetários
    public static decimal Valor(decimal min = 1, decimal max = 1000) =>
        Math.Round(_faker.Random.Decimal(min, max), 2);
    public static decimal ValorPagamento() => Math.Round(_faker.Random.Decimal(50, 500), 2);
    public static decimal ValorMensalidade() => Math.Round(_faker.Random.Decimal(100, 300), 2);

    // Datas
    public static DateTime DataPassada(int diasAtras = 30) =>
        _faker.Date.Past(yearsToGoBack: 0, refDate: DateTime.UtcNow.AddDays(-diasAtras));
    public static DateTime DataFutura(int diasFrente = 30) =>
        _faker.Date.Future(yearsToGoForward: 0, refDate: DateTime.UtcNow.AddDays(diasFrente));
    public static DateTime DataRecente() => _faker.Date.Recent(days: 7);

    // Dados específicos do domínio
    public static string NomeTurma() =>
        _faker.PickRandom("Turma de Violão", "Turma de Piano", "Turma de Guitarra", "Turma de Bateria", "Turma de Canto");
    public static int CapacidadeTurma() => _faker.Random.Int(5, 20);
    public static string Observacao() => _faker.Lorem.Sentence(wordCount: _faker.Random.Int(3, 10));

    // Enums aleatórios
    public static TipoPessoa TipoPessoaAleatorio() => _faker.PickRandom<TipoPessoa>();
    public static TipoCurso TipoCursoAleatorio() => _faker.PickRandom<TipoCurso>();
    public static StatusAluno StatusAlunoAleatorio() => _faker.PickRandom<StatusAluno>();
    public static StatusMatricula StatusMatriculaAleatorio() => _faker.PickRandom<StatusMatricula>();
}
