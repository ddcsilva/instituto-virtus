using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Turma;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Turmas;

public class ObterTurmasComVagasQuery : IRequest<Result<List<TurmaDto>>>
{
}

public class ObterTurmasComVagasQueryHandler : IRequestHandler<ObterTurmasComVagasQuery, Result<List<TurmaDto>>>
{
    private readonly ITurmaRepository _turmaRepository;
    private readonly IMapper _mapper;

    public ObterTurmasComVagasQueryHandler(ITurmaRepository turmaRepository, IMapper mapper)
    {
        _turmaRepository = turmaRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<TurmaDto>>> Handle(ObterTurmasComVagasQuery request, CancellationToken cancellationToken)
    {
        var turmas = await _turmaRepository.GetComVagasAsync(cancellationToken);
        var dto = _mapper.Map<List<TurmaDto>>(turmas);
        return Result<List<TurmaDto>>.Success(dto);
    }
}