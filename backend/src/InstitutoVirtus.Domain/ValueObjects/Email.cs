namespace InstitutoVirtus.Domain.ValueObjects;

public class Email : ValueObject
{
    public string Endereco { get; private set; }

    protected Email() { Endereco = string.Empty; }

    public Email(string endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco))
            throw new ArgumentException("Email não pode ser vazio");

        if (!ValidarEmail(endereco))
            throw new ArgumentException("Email inválido");

        Endereco = endereco.ToLower();
    }

    private static bool ValidarEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Endereco;
    }
}