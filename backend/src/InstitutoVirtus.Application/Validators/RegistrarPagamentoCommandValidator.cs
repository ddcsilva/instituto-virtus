using FluentValidation;
using InstitutoVirtus.Application.Commands.Pagamentos;
using InstitutoVirtus.Domain.Enums;

namespace InstitutoVirtus.Application.Validators;

public class RegistrarPagamentoCommandValidator : AbstractValidator<RegistrarPagamentoCommand>
{
    public RegistrarPagamentoCommandValidator()
    {
        RuleFor(x => x.ResponsavelId)
            .NotEmpty().WithMessage("Responsável é obrigatório");

        RuleFor(x => x.ValorTotal)
            .GreaterThan(0).WithMessage("Valor deve ser maior que zero");

        RuleFor(x => x.MeioPagamento)
            .NotEmpty().WithMessage("Meio de pagamento é obrigatório")
            .Must(BeValidPaymentMethod).WithMessage("Meio de pagamento inválido");

        RuleFor(x => x.Alocacoes)
            .NotEmpty().WithMessage("Deve alocar para pelo menos uma mensalidade");

        RuleForEach(x => x.Alocacoes)
            .ChildRules(alocacao =>
            {
                alocacao.RuleFor(a => a.ValorAlocado)
                    .GreaterThan(0).WithMessage("Valor alocado deve ser maior que zero");
            });
    }

    private bool BeValidPaymentMethod(string method)
    {
        return Enum.TryParse<MeioPagamento>(method, out _);
    }
}
