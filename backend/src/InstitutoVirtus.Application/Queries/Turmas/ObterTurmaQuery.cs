using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Turma;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Turmas;

public class ObterTurmaQuery : IRequest<Result<TurmaDto>>
{
    public Guid Id { get; set; }
}

public class ObterTurmaQueryHandler : IRequestHandler<ObterTurmaQuery, Result<TurmaDto>>
{
    private readonly ITurmaRepository _turmaRepository;
    private readonly IMapper _mapper;

    public ObterTurmaQueryHandler(ITurmaRepository turmaRepository, IMapper mapper)
    {
        _turmaRepository = turmaRepository;
        _mapper = mapper;
    }

    public async Task<Result<TurmaDto>> Handle(ObterTurmaQuery request, CancellationToken cancellationToken)
    {
        var turma = await _turmaRepository.GetByIdAsync(request.Id, cancellationToken);

        if (turma == null)
            return Result<TurmaDto>.Failure("Turma não encontrada");

        var dto = _mapper.Map<TurmaDto>(turma);
        return Result<TurmaDto>.Success(dto);
    }
}