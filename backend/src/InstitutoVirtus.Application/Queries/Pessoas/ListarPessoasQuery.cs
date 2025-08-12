namespace InstitutoVirtus.Application.Queries.Pessoas;

using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Pessoa;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

public class ListarPessoasQuery : IRequest<Result<PagedResult<PessoaDto>>>
{
    public string? Nome { get; set; }
    public string? Tipo { get; set; }
    public bool? Ativo { get; set; }
    public int Page { get; set; } = 0; // front usa 0-based
    public int PageSize { get; set; } = 10;
}

public class ListarPessoasQueryHandler : IRequestHandler<ListarPessoasQuery, Result<PagedResult<PessoaDto>>>
{
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IMapper _mapper;

    public ListarPessoasQueryHandler(IPessoaRepository pessoaRepository, IMapper mapper)
    {
        _pessoaRepository = pessoaRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<PessoaDto>>> Handle(ListarPessoasQuery request, CancellationToken cancellationToken)
    {
        TipoPessoa? tipo = null;
        if (!string.IsNullOrWhiteSpace(request.Tipo) && Enum.TryParse<TipoPessoa>(request.Tipo, out var parsed))
        {
            tipo = parsed;
        }

        // converter página 0-based do front para 1-based no repositório
        var pageNumber = request.Page + 1;
        var (items, total) = await _pessoaRepository.SearchAsync(
            request.Nome,
            tipo,
            request.Ativo,
            pageNumber,
            request.PageSize,
            cancellationToken);

        var dto = _mapper.Map<IEnumerable<PessoaDto>>(items).ToList();
        var result = new PagedResult<PessoaDto>(dto, total, request.Page, request.PageSize);
        return Result<PagedResult<PessoaDto>>.Success(result);
    }
}


