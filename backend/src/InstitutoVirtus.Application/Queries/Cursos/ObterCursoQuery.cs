namespace InstitutoVirtus.Application.Queries.Cursos;

using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Curso;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

public class ObterCursoQuery : IRequest<Result<CursoDto>>
{
    public Guid Id { get; set; }
}

public class ObterCursoQueryHandler : IRequestHandler<ObterCursoQuery, Result<CursoDto>>
{
    private readonly ICursoRepository _repo;
    private readonly IMapper _mapper;

    public ObterCursoQueryHandler(ICursoRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Result<CursoDto>> Handle(ObterCursoQuery request, CancellationToken cancellationToken)
    {
        var curso = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (curso == null) return Result<CursoDto>.Failure("Curso não encontrado");
        return Result<CursoDto>.Success(_mapper.Map<CursoDto>(curso));
    }
}


