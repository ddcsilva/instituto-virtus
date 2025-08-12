namespace InstitutoVirtus.Application.Commands.Cursos;

using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Curso;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

public class CriarCursoCommand : IRequest<Result<CursoDto>>
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal ValorMensalidade { get; set; }
    public int CargaHoraria { get; set; }
}

public class CriarCursoCommandHandler : IRequestHandler<CriarCursoCommand, Result<CursoDto>>
{
    private readonly ICursoRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CriarCursoCommandHandler(ICursoRepository repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<CursoDto>> Handle(CriarCursoCommand request, CancellationToken cancellationToken)
    {
        if (await _repo.ExistsAsync(c => c.Nome == request.Nome, cancellationToken))
            return Result<CursoDto>.Failure("Já existe curso com este nome");

        var curso = new Curso(request.Nome, request.Descricao, request.ValorMensalidade, request.CargaHoraria);
        await _repo.AddAsync(curso, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<CursoDto>.Success(_mapper.Map<CursoDto>(curso));
    }
}


