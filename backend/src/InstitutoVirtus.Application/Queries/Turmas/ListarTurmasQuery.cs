using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Turma;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Turmas;

public class ListarTurmasQuery : IRequest<Result<List<TurmaDto>>>
{
    public int AnoLetivo { get; set; }
    public int? Periodo { get; set; }
}

public class ListarTurmasQueryHandler : IRequestHandler<ListarTurmasQuery, Result<List<TurmaDto>>>
{
    private readonly ITurmaRepository _turmaRepository;
    private readonly IMapper _mapper;

    public ListarTurmasQueryHandler(ITurmaRepository turmaRepository, IMapper mapper)
    {
        _turmaRepository = turmaRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<TurmaDto>>> Handle(ListarTurmasQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Turma> turmas;

        if (request.Periodo.HasValue)
        {
            turmas = await _turmaRepository.GetByPeriodoAsync(request.AnoLetivo, request.Periodo.Value, cancellationToken);
        }
        else
        {
            turmas = await _turmaRepository.GetByAnoLetivoAsync(request.AnoLetivo, cancellationToken);
        }

        var dto = _mapper.Map<List<TurmaDto>>(turmas);
        return Result<List<TurmaDto>>.Success(dto);
    }
}