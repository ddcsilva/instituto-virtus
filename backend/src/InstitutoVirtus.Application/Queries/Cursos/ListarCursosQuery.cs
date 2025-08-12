namespace InstitutoVirtus.Application.Queries.Cursos;

using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Curso;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

public class ListarCursosQuery : IRequest<Result<PagedResult<CursoDto>>>
{
    public string? Nome { get; set; }
    public bool? Ativo { get; set; }
    public int Page { get; set; } = 0; // 0-based
    public int PageSize { get; set; } = 10;
}

public class ListarCursosQueryHandler : IRequestHandler<ListarCursosQuery, Result<PagedResult<CursoDto>>>
{
    private readonly ICursoRepository _repo;
    private readonly IMapper _mapper;

    public ListarCursosQueryHandler(ICursoRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<CursoDto>>> Handle(ListarCursosQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _repo.SearchAsync(request.Nome, request.Ativo, request.Page, request.PageSize, cancellationToken);
        var dtos = _mapper.Map<List<CursoDto>>(items);
        return Result<PagedResult<CursoDto>>.Success(new PagedResult<CursoDto>(dtos, total, request.Page, request.PageSize));
    }
}


