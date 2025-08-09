namespace InstitutoVirtus.Domain.ValueObjects;

public class Competencia : ValueObject
{
    public int Ano { get; private set; }
    public int Mes { get; private set; }

    protected Competencia() { }

    public Competencia(int ano, int mes)
    {
        if (mes < 1 || mes > 12)
            throw new ArgumentException("Mês deve estar entre 1 e 12");

        if (ano < 2020 || ano > 2100)
            throw new ArgumentException("Ano inválido");

        Ano = ano;
        Mes = mes;
    }

    public static Competencia Atual()
    {
        var hoje = DateTime.Today;
        return new Competencia(hoje.Year, hoje.Month);
    }

    public string FormatoString()
    {
        return $"{Ano:0000}-{Mes:00}";
    }

    public DateTime PrimeiraData()
    {
        return new DateTime(Ano, Mes, 1);
    }

    public DateTime UltimaData()
    {
        return new DateTime(Ano, Mes, DateTime.DaysInMonth(Ano, Mes));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Ano;
        yield return Mes;
    }
}