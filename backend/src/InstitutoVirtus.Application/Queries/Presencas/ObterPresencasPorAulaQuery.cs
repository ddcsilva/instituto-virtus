using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Presenca;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Presencas;

public class ObterPresencasPorAulaQuery : IRequest<Result<List<PresencaDto>>>
{
    public Guid AulaId { get; set; }
}

public class ObterPresencasPorAulaQueryHandler : IRequestHandler<ObterPresencasPorAulaQuery, Result<List<PresencaDto>>>
{
    private readonly IAulaRepository _aulaRepository;
    private readonly IMapper _mapper;

    public ObterPresencasPorAulaQueryHandler(
        IAulaRepository aulaRepository,
        IMapper mapper)
    {
        _aulaRepository = aulaRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<PresencaDto>>> Handle(ObterPresencasPorAulaQuery request, CancellationToken cancellationToken)
    {
        var aula = await _aulaRepository.GetByIdAsync(request.AulaId, cancellationToken);

        if (aula == null)
            return Result<List<PresencaDto>>.Failure("Aula não encontrada");

        var dto = _mapper.Map<List<PresencaDto>>(aula.Presencas);
        return Result<List<PresencaDto>>.Success(dto);
    }
}