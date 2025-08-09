using FluentValidation;
using InstitutoVirtus.Application.Commands.Matriculas;

public class RealizarMatriculaCommandValidator : AbstractValidator<RealizarMatriculaCommand>
{
    public RealizarMatriculaCommandValidator()
    {
        RuleFor(x => x.AlunoId)
            .NotEmpty().WithMessage("Aluno é obrigatório");

        RuleFor(x => x.TurmaId)
            .NotEmpty().WithMessage("Turma é obrigatória");

        RuleFor(x => x.MesesQuantidade)
            .GreaterThan(0).WithMessage("Quantidade de meses deve ser maior que zero")
            .LessThanOrEqualTo(12).WithMessage("Quantidade máxima de 12 meses");

        RuleFor(x => x.DiaVencimento)
            .InclusiveBetween(1, 28).WithMessage("Dia de vencimento deve estar entre 1 e 28");
    }
}
