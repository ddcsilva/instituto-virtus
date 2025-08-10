using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Mensalidade;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Mensalidades;

public class ListarMensalidadesQuery : IRequest<Result<List<MensalidadeDto>>>
{
    public int? Ano { get; set; }
    public int? Mes { get; set; }
    public StatusMensalidade? Status { get; set; }
}

public class ListarMensalidadesQueryHandler : IRequestHandler<ListarMensalidadesQuery, Result<List<MensalidadeDto>>>
{
    private readonly IMensalidadeRepository _mensalidadeRepository;
    private readonly IMapper _mapper;

    public ListarMensalidadesQueryHandler(
        IMensalidadeRepository mensalidadeRepository,
        IMapper mapper)
    {
        _mensalidadeRepository = mensalidadeRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<MensalidadeDto>>> Handle(ListarMensalidadesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Mensalidade> mensalidades;

        if (request.Ano.HasValue && request.Mes.HasValue)
        {
            mensalidades = await _mensalidadeRepository.GetByCompetenciaAsync(request.Ano.Value, request.Mes.Value, cancellationToken);
        }
        else
        {
            mensalidades = await _mensalidadeRepository.GetAllAsync(cancellationToken);
        }

        if (request.Status.HasValue)
        {
            mensalidades = mensalidades.Where(m => m.Status == request.Status.Value);
        }

        var dto = _mapper.Map<List<MensalidadeDto>>(mensalidades);
        return Result<List<MensalidadeDto>>.Success(dto);
    }
}