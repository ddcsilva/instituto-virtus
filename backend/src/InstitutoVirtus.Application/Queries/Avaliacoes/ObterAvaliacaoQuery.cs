using MediatR;
using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Avaliacao;
using InstitutoVirtus.Domain.Interfaces.Repositories;

namespace InstitutoVirtus.Application.Queries.Avaliacoes;

public class ObterAvaliacaoQuery : IRequest<Result<AvaliacaoDto>>
{
    public Guid Id { get; set; }
}

public class ObterAvaliacaoQueryHandler : IRequestHandler<ObterAvaliacaoQuery, Result<AvaliacaoDto>>
{
    private readonly IAvaliacaoRepository _avaliacaoRepository;
    private readonly IMapper _mapper;

    public ObterAvaliacaoQueryHandler(
        IAvaliacaoRepository avaliacaoRepository,
        IMapper mapper)
    {
        _avaliacaoRepository = avaliacaoRepository;
        _mapper = mapper;
    }

    public async Task<Result<AvaliacaoDto>> Handle(ObterAvaliacaoQuery request, CancellationToken cancellationToken)
    {
        var avaliacao = await _avaliacaoRepository.GetByIdAsync(request.Id, cancellationToken);

        if (avaliacao == null)
            return Result<AvaliacaoDto>.Failure("Avaliação não encontrada");

        var dto = _mapper.Map<AvaliacaoDto>(avaliacao);
        return Result<AvaliacaoDto>.Success(dto);
    }
}