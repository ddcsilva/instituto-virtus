namespace InstitutoVirtus.Domain.ValueObjects;

public class Cpf : ValueObject
{
    public string Numero { get; private set; }

    protected Cpf() { Numero = string.Empty; }

    public Cpf(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("CPF é obrigatório");

        var limpo = new string(numero.Where(char.IsDigit).ToArray());
        if (limpo.Length != 11 || !ValidarCpf(limpo))
            throw new ArgumentException("CPF inválido");

        Numero = limpo;
    }

    private static bool ValidarCpf(string cpf)
    {
        // Regras básicas de validação de CPF
        if (cpf.Distinct().Count() == 1) return false;

        int[] mult1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] mult2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf[..9];
        int soma = 0;
        for (int i = 0; i < 9; i++) soma += int.Parse(tempCpf[i].ToString()) * mult1[i];
        int resto = soma % 11;
        int dig1 = resto < 2 ? 0 : 11 - resto;

        tempCpf += dig1.ToString();
        soma = 0;
        for (int i = 0; i < 10; i++) soma += int.Parse(tempCpf[i].ToString()) * mult2[i];
        resto = soma % 11;
        int dig2 = resto < 2 ? 0 : 11 - resto;

        return cpf.EndsWith(dig1.ToString() + dig2.ToString());
    }

    public string NumeroFormatado()
    {
        return $"{Numero[..3]}.{Numero[3..6]}.{Numero[6..9]}-{Numero[9..]}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Numero;
    }
}


