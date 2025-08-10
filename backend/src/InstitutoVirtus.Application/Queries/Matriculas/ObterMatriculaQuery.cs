using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Matricula;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Matriculas;

public class ObterMatriculaQuery : IRequest<Result<MatriculaDto>>
{
    public Guid Id { get; set; }
}

public class ObterMatriculaQueryHandler : IRequestHandler<ObterMatriculaQuery, Result<MatriculaDto>>
{
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IMapper _mapper;

    public ObterMatriculaQueryHandler(
        IMatriculaRepository matriculaRepository,
        IMapper mapper)
    {
        _matriculaRepository = matriculaRepository;
        _mapper = mapper;
    }

    public async Task<Result<MatriculaDto>> Handle(ObterMatriculaQuery request, CancellationToken cancellationToken)
    {
        var matricula = await _matriculaRepository.GetByIdAsync(request.Id, cancellationToken);

        if (matricula == null)
            return Result<MatriculaDto>.Failure("Matrícula não encontrada");

        var dto = _mapper.Map<MatriculaDto>(matricula);
        return Result<MatriculaDto>.Success(dto);
    }
}