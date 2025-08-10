using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Pagamento;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Pagamentos;

public class ObterPagamentosPorResponsavelQuery : IRequest<Result<List<PagamentoDto>>>
{
    public Guid ResponsavelId { get; set; }
}

public class ObterPagamentosPorResponsavelQueryHandler : IRequestHandler<ObterPagamentosPorResponsavelQuery, Result<List<PagamentoDto>>>
{
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly IMapper _mapper;

    public ObterPagamentosPorResponsavelQueryHandler(
        IPagamentoRepository pagamentoRepository,
        IMapper mapper)
    {
        _pagamentoRepository = pagamentoRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<PagamentoDto>>> Handle(ObterPagamentosPorResponsavelQuery request, CancellationToken cancellationToken)
    {
        var pagamentos = await _pagamentoRepository.GetByResponsavelAsync(request.ResponsavelId, cancellationToken);
        var dto = _mapper.Map<List<PagamentoDto>>(pagamentos);
        return Result<List<PagamentoDto>>.Success(dto);
    }
}