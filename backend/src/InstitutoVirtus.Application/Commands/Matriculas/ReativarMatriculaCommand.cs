using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Commands.Matriculas;

public class ReativarMatriculaCommand : IRequest<Result>
{
    public Guid MatriculaId { get; set; }
}

public class ReativarMatriculaCommandHandler : IRequestHandler<ReativarMatriculaCommand, Result>
{
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly ITurmaRepository _turmaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReativarMatriculaCommandHandler(
        IMatriculaRepository matriculaRepository,
        ITurmaRepository turmaRepository,
        IUnitOfWork unitOfWork)
    {
        _matriculaRepository = matriculaRepository;
        _turmaRepository = turmaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReativarMatriculaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var matricula = await _matriculaRepository.GetByIdAsync(request.MatriculaId, cancellationToken);

            if (matricula == null)
                return Result.Failure("Matrícula não encontrada");

            // Verificar se ainda há vaga na turma
            var turma = await _turmaRepository.GetByIdAsync(matricula.TurmaId, cancellationToken);
            if (turma == null || !turma.TemVaga())
                return Result.Failure("Turma sem vagas disponíveis");

            matricula.Reativar();

            await _matriculaRepository.UpdateAsync(matricula, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao reativar matrícula: {ex.Message}");
        }
    }
}