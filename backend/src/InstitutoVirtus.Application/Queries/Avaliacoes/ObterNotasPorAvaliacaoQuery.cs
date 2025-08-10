using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Nota;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Avaliacoes;

public class ObterNotasPorAvaliacaoQuery : IRequest<Result<List<NotaDto>>>
{
    public Guid AvaliacaoId { get; set; }
}

public class ObterNotasPorAvaliacaoQueryHandler : IRequestHandler<ObterNotasPorAvaliacaoQuery, Result<List<NotaDto>>>
{
    private readonly IAvaliacaoRepository _avaliacaoRepository;
    private readonly IMapper _mapper;

    public ObterNotasPorAvaliacaoQueryHandler(
        IAvaliacaoRepository avaliacaoRepository,
        IMapper mapper)
    {
        _avaliacaoRepository = avaliacaoRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<NotaDto>>> Handle(ObterNotasPorAvaliacaoQuery request, CancellationToken cancellationToken)
    {
        var avaliacao = await _avaliacaoRepository.GetByIdAsync(request.AvaliacaoId, cancellationToken);

        if (avaliacao == null)
            return Result<List<NotaDto>>.Failure("Avaliação não encontrada");

        var dto = _mapper.Map<List<NotaDto>>(avaliacao.Notas);
        return Result<List<NotaDto>>.Success(dto);
    }
}