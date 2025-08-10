using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Pagamento;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Pagamentos;

public class ObterPagamentoQuery : IRequest<Result<PagamentoDto>>
{
    public Guid Id { get; set; }
}

public class ObterPagamentoQueryHandler : IRequestHandler<ObterPagamentoQuery, Result<PagamentoDto>>
{
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly IMapper _mapper;

    public ObterPagamentoQueryHandler(
        IPagamentoRepository pagamentoRepository,
        IMapper mapper)
    {
        _pagamentoRepository = pagamentoRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagamentoDto>> Handle(ObterPagamentoQuery request, CancellationToken cancellationToken)
    {
        var pagamento = await _pagamentoRepository.GetByIdAsync(request.Id, cancellationToken);

        if (pagamento == null)
            return Result<PagamentoDto>.Failure("Pagamento não encontrado");

        var dto = _mapper.Map<PagamentoDto>(pagamento);
        return Result<PagamentoDto>.Success(dto);
    }
}