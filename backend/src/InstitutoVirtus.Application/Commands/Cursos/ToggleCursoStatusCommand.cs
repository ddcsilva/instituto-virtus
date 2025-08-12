namespace InstitutoVirtus.Application.Commands.Cursos;

using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Curso;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

public class ToggleCursoStatusCommand : IRequest<Result<CursoDto>>
{
    public Guid Id { get; set; }
}

public class ToggleCursoStatusCommandHandler : IRequestHandler<ToggleCursoStatusCommand, Result<CursoDto>>
{
    private readonly ICursoRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ToggleCursoStatusCommandHandler(ICursoRepository repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<CursoDto>> Handle(ToggleCursoStatusCommand request, CancellationToken cancellationToken)
    {
        var curso = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (curso == null) return Result<CursoDto>.Failure("Curso não encontrado");
        if (curso.Ativo) curso.Desativar(); else curso.Ativar();
        await _repo.UpdateAsync(curso, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Result<CursoDto>.Success(_mapper.Map<CursoDto>(curso));
    }
}


