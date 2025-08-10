using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Matricula;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Matriculas;

public class ListarMatriculasQuery : IRequest<Result<List<MatriculaDto>>>
{
    public StatusMatricula? Status { get; set; }
    public Guid? TurmaId { get; set; }
    public Guid? AlunoId { get; set; }
}

public class ListarMatriculasQueryHandler : IRequestHandler<ListarMatriculasQuery, Result<List<MatriculaDto>>>
{
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IMapper _mapper;

    public ListarMatriculasQueryHandler(
        IMatriculaRepository matriculaRepository,
        IMapper mapper)
    {
        _matriculaRepository = matriculaRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<MatriculaDto>>> Handle(ListarMatriculasQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Matricula> matriculas;

        if (request.AlunoId.HasValue)
        {
            matriculas = await _matriculaRepository.GetByAlunoAsync(request.AlunoId.Value, cancellationToken);
        }
        else if (request.TurmaId.HasValue)
        {
            matriculas = await _matriculaRepository.GetByTurmaAsync(request.TurmaId.Value, cancellationToken);
        }
        else
        {
            matriculas = await _matriculaRepository.GetAllAsync(cancellationToken);
        }

        if (request.Status.HasValue)
        {
            matriculas = matriculas.Where(m => m.Status == request.Status.Value);
        }

        var dto = _mapper.Map<List<MatriculaDto>>(matriculas);
        return Result<List<MatriculaDto>>.Success(dto);
    }
}