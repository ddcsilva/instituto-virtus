using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Turmas;

public class ObterGradeHorariosQuery : IRequest<Result<GradeHorariosDto>>
{
    public Guid? TurmaId { get; set; }
    public Guid? ProfessorId { get; set; }
}

public class GradeHorariosDto
{
    public List<HorarioDto> Horarios { get; set; } = new();
}

public class HorarioDto
{
    public string DiaSemana { get; set; } = string.Empty;
    public string Horario { get; set; } = string.Empty;
    public string Turma { get; set; } = string.Empty;
    public string Professor { get; set; } = string.Empty;
    public string Sala { get; set; } = string.Empty;
}

public class ObterGradeHorariosQueryHandler : IRequestHandler<ObterGradeHorariosQuery, Result<GradeHorariosDto>>
{
    private readonly ITurmaRepository _turmaRepository;

    public ObterGradeHorariosQueryHandler(ITurmaRepository turmaRepository)
    {
        _turmaRepository = turmaRepository;
    }

    public async Task<Result<GradeHorariosDto>> Handle(ObterGradeHorariosQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Turma> turmas;

        if (request.TurmaId.HasValue)
        {
            var turma = await _turmaRepository.GetByIdAsync(request.TurmaId.Value, cancellationToken);
            turmas = turma != null ? new[] { turma } : Array.Empty<Turma>();
        }
        else if (request.ProfessorId.HasValue)
        {
            turmas = await _turmaRepository.GetByProfessorAsync(request.ProfessorId.Value, cancellationToken);
        }
        else
        {
            turmas = await _turmaRepository.GetAllAsync(cancellationToken);
        }

        var horarios = turmas.Select(t => new HorarioDto
        {
            DiaSemana = t.DiaSemana.ToString(),
            Horario = t.Horario.FormatoString(),
            Turma = t.ObterNome(),
            Professor = t.Professor?.NomeCompleto ?? "",
            Sala = t.Sala ?? ""
        }).ToList();

        var result = new GradeHorariosDto { Horarios = horarios };
        return Result<GradeHorariosDto>.Success(result);
    }
}