using FluentValidation;
using InstitutoVirtus.Application.Commands.Turmas;

namespace InstitutoVirtus.Application.Validators;

public class CriarTurmaCommandValidator : AbstractValidator<CriarTurmaCommand>
{
    public CriarTurmaCommandValidator()
    {
        RuleFor(x => x.CursoId)
            .NotEmpty().WithMessage("Curso é obrigatório");

        RuleFor(x => x.ProfessorId)
            .NotEmpty().WithMessage("Professor é obrigatório");

        RuleFor(x => x.DiaSemana)
            .NotEmpty().WithMessage("Dia da semana é obrigatório")
            .Must(BeValidDayOfWeek).WithMessage("Dia da semana inválido");

        RuleFor(x => x.HoraInicio)
            .LessThan(x => x.HoraFim).WithMessage("Hora início deve ser menor que hora fim");

        RuleFor(x => x)
            .Must(x => (x.HoraFim - x.HoraInicio).TotalMinutes == 50)
            .WithMessage("Aula deve ter duração de 50 minutos");

        RuleFor(x => x.Capacidade)
            .GreaterThan(0).WithMessage("Capacidade deve ser maior que zero")
            .LessThanOrEqualTo(50).WithMessage("Capacidade máxima é 50 alunos");

        RuleFor(x => x.AnoLetivo)
            .InclusiveBetween(2025, 2030).WithMessage("Ano letivo inválido");

        RuleFor(x => x.Periodo)
            .InclusiveBetween(1, 2).WithMessage("Período deve ser 1 ou 2");
    }

    private bool BeValidDayOfWeek(string dayOfWeek)
    {
        var validDays = new[] { "Segunda", "Terca", "Quarta", "Quinta", "Sexta", "Sabado", "Domingo" };
        return validDays.Contains(dayOfWeek);
    }
}