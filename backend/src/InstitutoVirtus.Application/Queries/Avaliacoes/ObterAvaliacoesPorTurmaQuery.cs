using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Avaliacao;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Avaliacoes;

public class ObterAvaliacoesPorTurmaQuery : IRequest<Result<List<AvaliacaoDto>>>
{
    public Guid TurmaId { get; set; }
}

public class ObterAvaliacoesPorTurmaQueryHandler : IRequestHandler<ObterAvaliacoesPorTurmaQuery, Result<List<AvaliacaoDto>>>
{
    private readonly IAvaliacaoRepository _avaliacaoRepository;
    private readonly IMapper _mapper;

    public ObterAvaliacoesPorTurmaQueryHandler(
        IAvaliacaoRepository avaliacaoRepository,
        IMapper mapper)
    {
        _avaliacaoRepository = avaliacaoRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<AvaliacaoDto>>> Handle(ObterAvaliacoesPorTurmaQuery request, CancellationToken cancellationToken)
    {
        var avaliacoes = await _avaliacaoRepository.GetByTurmaAsync(request.TurmaId, cancellationToken);
        var dto = _mapper.Map<List<AvaliacaoDto>>(avaliacoes);
        return Result<List<AvaliacaoDto>>.Success(dto);
    }
}
