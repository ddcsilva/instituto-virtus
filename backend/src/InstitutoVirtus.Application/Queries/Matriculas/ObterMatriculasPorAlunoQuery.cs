using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Matricula;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Matriculas;

public class ObterMatriculasPorAlunoQuery : IRequest<Result<List<MatriculaDto>>>
{
    public Guid AlunoId { get; set; }
}

public class ObterMatriculasPorAlunoQueryHandler : IRequestHandler<ObterMatriculasPorAlunoQuery, Result<List<MatriculaDto>>>
{
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IMapper _mapper;

    public ObterMatriculasPorAlunoQueryHandler(
        IMatriculaRepository matriculaRepository,
        IMapper mapper)
    {
        _matriculaRepository = matriculaRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<MatriculaDto>>> Handle(ObterMatriculasPorAlunoQuery request, CancellationToken cancellationToken)
    {
        var matriculas = await _matriculaRepository.GetByAlunoAsync(request.AlunoId, cancellationToken);
        var dto = _mapper.Map<List<MatriculaDto>>(matriculas);
        return Result<List<MatriculaDto>>.Success(dto);
    }
}