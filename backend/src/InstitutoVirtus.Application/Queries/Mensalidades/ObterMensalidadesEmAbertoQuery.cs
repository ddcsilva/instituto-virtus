using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Mensalidade;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Mensalidades;

public class ObterMensalidadesEmAbertoQuery : IRequest<Result<List<MensalidadeDto>>>
{
    public Guid ResponsavelId { get; set; }
}

public class ObterMensalidadesEmAbertoQueryHandler : IRequestHandler<ObterMensalidadesEmAbertoQuery, Result<List<MensalidadeDto>>>
{
    private readonly IMensalidadeRepository _mensalidadeRepository;
    private readonly IMapper _mapper;

    public ObterMensalidadesEmAbertoQueryHandler(
        IMensalidadeRepository mensalidadeRepository,
        IMapper mapper)
    {
        _mensalidadeRepository = mensalidadeRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<MensalidadeDto>>> Handle(ObterMensalidadesEmAbertoQuery request, CancellationToken cancellationToken)
    {
        var mensalidades = await _mensalidadeRepository.GetEmAbertoByResponsavelAsync(request.ResponsavelId, cancellationToken);
        var dto = _mapper.Map<List<MensalidadeDto>>(mensalidades);

        return Result<List<MensalidadeDto>>.Success(dto);
    }
}
