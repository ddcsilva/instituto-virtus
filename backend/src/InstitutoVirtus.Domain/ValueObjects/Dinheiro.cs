namespace InstitutoVirtus.Domain.ValueObjects;

public class Dinheiro : ValueObject
{
    public decimal Valor { get; private set; }

    protected Dinheiro() { }

    public Dinheiro(decimal valor)
    {
        if (valor < 0)
            throw new ArgumentException("Valor não pode ser negativo");

        Valor = Math.Round(valor, 2);
    }

    public static Dinheiro Zero => new(0);

    public static Dinheiro operator +(Dinheiro a, Dinheiro b)
        => new(a.Valor + b.Valor);

    public static Dinheiro operator -(Dinheiro a, Dinheiro b)
        => new(a.Valor - b.Valor);

    public static bool operator >(Dinheiro a, Dinheiro b)
        => a.Valor > b.Valor;

    public static bool operator <(Dinheiro a, Dinheiro b)
        => a.Valor < b.Valor;

    public static bool operator >=(Dinheiro a, Dinheiro b)
        => a.Valor >= b.Valor;

    public static bool operator <=(Dinheiro a, Dinheiro b)
        => a.Valor <= b.Valor;

    public string FormatoMoeda()
    {
        return $"R$ {Valor:N2}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor;
    }
}