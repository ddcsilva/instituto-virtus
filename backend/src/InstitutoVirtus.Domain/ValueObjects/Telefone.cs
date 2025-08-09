namespace InstitutoVirtus.Domain.ValueObjects;

public class Telefone : ValueObject
{
    public string Numero { get; private set; }

    protected Telefone() { }

    public Telefone(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Telefone é obrigatório");

        var numeroLimpo = LimparNumero(numero);

        if (!ValidarTelefone(numeroLimpo))
            throw new ArgumentException("Telefone inválido");

        Numero = numeroLimpo;
    }

    private static string LimparNumero(string numero)
    {
        return new string(numero.Where(char.IsDigit).ToArray());
    }

    private static bool ValidarTelefone(string numero)
    {
        return numero.Length >= 10 && numero.Length <= 11;
    }

    public string NumeroFormatado()
    {
        if (Numero.Length == 11)
            return $"({Numero[..2]}) {Numero[2]} {Numero[3..7]}-{Numero[7..]}";
        return $"({Numero[..2]}) {Numero[2..6]}-{Numero[6..]}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Numero;
    }
}