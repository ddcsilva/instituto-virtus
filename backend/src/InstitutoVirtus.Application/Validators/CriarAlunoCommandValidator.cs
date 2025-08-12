namespace InstitutoVirtus.Application.Validators;

using FluentValidation;
using InstitutoVirtus.Application.Commands.Pessoas;

public class CriarAlunoCommandValidator : AbstractValidator<CriarAlunoCommand>
{
    public CriarAlunoCommandValidator()
    {
        RuleFor(x => x.NomeCompleto)
            .NotEmpty().WithMessage("Nome completo é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter pelo menos 3 caracteres");

        RuleFor(x => x.Telefone)
            .NotEmpty().WithMessage("Telefone é obrigatório")
            .Matches(@"^\d{10,11}$").WithMessage("Telefone deve ter 10 ou 11 dígitos");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Email inválido");

        RuleFor(x => x.Cpf)
            .Matches(@"^\d{11}$").When(x => !string.IsNullOrWhiteSpace(x.Cpf))
            .WithMessage("CPF deve ter 11 dígitos");

        RuleFor(x => x.DataNascimento)
            .LessThan(DateTime.Today).WithMessage("Data de nascimento deve ser no passado")
            .GreaterThan(DateTime.Today.AddYears(-120)).WithMessage("Data de nascimento inválida");

        RuleFor(x => x)
            .Must(x => CalcularIdade(x.DataNascimento) >= 7)
            .WithMessage("Aluno deve ter pelo menos 7 anos");
    }

    private int CalcularIdade(DateTime dataNascimento)
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - dataNascimento.Year;
        if (dataNascimento.Date > hoje.AddYears(-idade))
            idade--;
        return idade;
    }
}
