using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Turma;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Domain.ValueObjects;
using MediatR;

namespace InstitutoVirtus.Application.Commands.Turmas;

public class CriarTurmaCommand : IRequest<Result<TurmaDto>>
{
    public Guid CursoId { get; set; }
    public Guid ProfessorId { get; set; }
    public string DiaSemana { get; set; } = string.Empty;
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFim { get; set; }
    public int Capacidade { get; set; }
    public string? Sala { get; set; }
    public int AnoLetivo { get; set; }
    public int Periodo { get; set; }
}

public class CriarTurmaCommandHandler : IRequestHandler<CriarTurmaCommand, Result<TurmaDto>>
{
    private readonly ITurmaRepository _turmaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CriarTurmaCommandHandler(
        ITurmaRepository turmaRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _turmaRepository = turmaRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TurmaDto>> Handle(CriarTurmaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var diaSemana = Enum.Parse<DiaSemana>(request.DiaSemana);

            // Verificar conflito de horário
            if (await _turmaRepository.ExisteConflitoHorarioAsync(request.ProfessorId, diaSemana, request.HoraInicio, cancellationToken))
                return Result<TurmaDto>.Failure("Professor já tem aula neste horário");

            var horario = new HorarioAula(request.HoraInicio, request.HoraFim);

            var turma = new Turma(
                request.CursoId,
                request.ProfessorId,
                diaSemana,
                horario,
                request.Capacidade,
                request.AnoLetivo,
                request.Periodo,
                request.Sala);

            await _turmaRepository.AddAsync(turma, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<TurmaDto>(turma);
            return Result<TurmaDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<TurmaDto>.Failure($"Erro ao criar turma: {ex.Message}");
        }
    }
}