using Bogus;

namespace Virtus.Domain.Tests.Extensions;

public static class FakerExtensions
{
    private static readonly Faker _faker = new("pt_BR");

    public static string NomeCompleto() => _faker.Name.FullName();
    public static string Email() => _faker.Internet.Email();
    public static string Telefone() => _faker.Phone.PhoneNumber("(##) #####-####");
    public static decimal Valor(decimal min = 1, decimal max = 1000) => _faker.Random.Decimal(min, max);
    public static DateTime DataPassada() => _faker.Date.Past();
    public static DateTime DataFutura() => _faker.Date.Future();
}
