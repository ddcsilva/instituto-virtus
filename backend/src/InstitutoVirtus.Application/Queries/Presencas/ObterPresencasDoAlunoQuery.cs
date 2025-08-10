using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Presenca;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Presencas;

public class ObterPresencasDoAlunoQuery : IRequest<Result<List<PresencaDto>>>
{
    public Guid AlunoId { get; set; }
    public Guid TurmaId { get; set; }
}

public class ObterPresencasDoAlunoQueryHandler : IRequestHandler<ObterPresencasDoAlunoQuery, Result<List<PresencaDto>>>
{
    private readonly IAulaRepository _aulaRepository;
    private readonly IMapper _mapper;

    public ObterPresencasDoAlunoQueryHandler(
        IAulaRepository aulaRepository,
        IMapper mapper)
    {
        _aulaRepository = aulaRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<PresencaDto>>> Handle(ObterPresencasDoAlunoQuery request, CancellationToken cancellationToken)
    {
        var aulas = await _aulaRepository.GetByTurmaAsync(request.TurmaId, cancellationToken);
        var presencas = aulas
            .SelectMany(a => a.Presencas)
            .Where(p => p.AlunoId == request.AlunoId)
            .ToList();

        var dto = _mapper.Map<List<PresencaDto>>(presencas);
        return Result<List<PresencaDto>>.Success(dto);
    }
}