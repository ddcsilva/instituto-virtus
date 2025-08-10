using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Pagamento;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Pagamentos;

public class ListarPagamentosQuery : IRequest<Result<List<PagamentoDto>>>
{
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
}

public class ListarPagamentosQueryHandler : IRequestHandler<ListarPagamentosQuery, Result<List<PagamentoDto>>>
{
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly IMapper _mapper;

    public ListarPagamentosQueryHandler(
        IPagamentoRepository pagamentoRepository,
        IMapper mapper)
    {
        _pagamentoRepository = pagamentoRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<PagamentoDto>>> Handle(ListarPagamentosQuery request, CancellationToken cancellationToken)
    {
        var pagamentos = await _pagamentoRepository.GetByPeriodoAsync(request.DataInicio, request.DataFim, cancellationToken);
        var dto = _mapper.Map<List<PagamentoDto>>(pagamentos);
        return Result<List<PagamentoDto>>.Success(dto);
    }
}