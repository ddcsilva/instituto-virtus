using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Turma;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;
using AutoMapper;

namespace InstitutoVirtus.Application.Commands.Turmas;

public class ToggleTurmaStatusCommand : IRequest<Result<TurmaDto>>
{
    public Guid Id { get; set; }
}

public class ToggleTurmaStatusCommandHandler : IRequestHandler<ToggleTurmaStatusCommand, Result<TurmaDto>>
{
    private readonly ITurmaRepository _turmaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ToggleTurmaStatusCommandHandler(ITurmaRepository turmaRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _turmaRepository = turmaRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TurmaDto>> Handle(ToggleTurmaStatusCommand request, CancellationToken cancellationToken)
    {
        var turma = await _turmaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (turma == null)
            return Result<TurmaDto>.Failure("Turma não encontrada.");

        if (turma.Ativo) turma.Desativar(); else turma.Ativar();
        await _turmaRepository.UpdateAsync(turma, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<TurmaDto>(turma);
        return Result<TurmaDto>.Success(dto);
    }
}


