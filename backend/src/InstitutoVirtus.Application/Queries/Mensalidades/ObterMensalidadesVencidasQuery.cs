using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Mensalidade;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Mensalidades;

public class ObterMensalidadesVencidasQuery : IRequest<Result<List<MensalidadeDto>>>
{
}

public class ObterMensalidadesVencidasQueryHandler : IRequestHandler<ObterMensalidadesVencidasQuery, Result<List<MensalidadeDto>>>
{
    private readonly IMensalidadeRepository _mensalidadeRepository;
    private readonly IMapper _mapper;

    public ObterMensalidadesVencidasQueryHandler(
        IMensalidadeRepository mensalidadeRepository,
        IMapper mapper)
    {
        _mensalidadeRepository = mensalidadeRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<MensalidadeDto>>> Handle(ObterMensalidadesVencidasQuery request, CancellationToken cancellationToken)
    {
        var mensalidades = await _mensalidadeRepository.GetVencidasAsync(cancellationToken);
        var dto = _mapper.Map<List<MensalidadeDto>>(mensalidades);
        return Result<List<MensalidadeDto>>.Success(dto);
    }
}