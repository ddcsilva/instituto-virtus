using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Turma;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Commands.Turmas;

public class AtualizarTurmaCommand : IRequest<Result<TurmaDto>>
{
    public Guid Id { get; set; }
    public int Capacidade { get; set; }
    public string? Sala { get; set; }
    public bool Ativo { get; set; }
}

public class AtualizarTurmaCommandHandler : IRequestHandler<AtualizarTurmaCommand, Result<TurmaDto>>
{
    private readonly ITurmaRepository _turmaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AtualizarTurmaCommandHandler(
        ITurmaRepository turmaRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _turmaRepository = turmaRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TurmaDto>> Handle(AtualizarTurmaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var turma = await _turmaRepository.GetByIdAsync(request.Id, cancellationToken);

            if (turma == null)
                return Result<TurmaDto>.Failure("Turma não encontrada");

            // Atualizar propriedades permitidas
            turma.Capacidade = request.Capacidade;
            turma.Sala = request.Sala;

            if (request.Ativo)
                turma.Ativar();
            else
                turma.Desativar();

            await _turmaRepository.UpdateAsync(turma, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<TurmaDto>(turma);
            return Result<TurmaDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<TurmaDto>.Failure($"Erro ao atualizar turma: {ex.Message}");
        }
    }
}