namespace InstitutoVirtus.Application.Commands.Cursos;

using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Curso;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

public class AtualizarCursoCommand : IRequest<Result<CursoDto>>
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal ValorMensalidade { get; set; }
    public int CargaHoraria { get; set; }
    public bool Ativo { get; set; } = true;
}

public class AtualizarCursoCommandHandler : IRequestHandler<AtualizarCursoCommand, Result<CursoDto>>
{
    private readonly ICursoRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AtualizarCursoCommandHandler(ICursoRepository repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<CursoDto>> Handle(AtualizarCursoCommand request, CancellationToken cancellationToken)
    {
        var curso = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (curso == null) return Result<CursoDto>.Failure("Curso não encontrado");

        curso.AtualizarDados(request.Nome, request.Descricao, request.ValorMensalidade, request.CargaHoraria);
        if (request.Ativo) curso.Ativar(); else curso.Desativar();
        await _repo.UpdateAsync(curso, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<CursoDto>.Success(_mapper.Map<CursoDto>(curso));
    }
}


