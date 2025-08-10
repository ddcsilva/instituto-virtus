using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Commands.Matriculas;

public class TrancarMatriculaCommand : IRequest<Result>
{
    public Guid MatriculaId { get; set; }
    public string Motivo { get; set; } = string.Empty;
}

public class TrancarMatriculaCommandHandler : IRequestHandler<TrancarMatriculaCommand, Result>
{
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TrancarMatriculaCommandHandler(
        IMatriculaRepository matriculaRepository,
        IUnitOfWork unitOfWork)
    {
        _matriculaRepository = matriculaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(TrancarMatriculaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var matricula = await _matriculaRepository.GetByIdAsync(request.MatriculaId, cancellationToken);

            if (matricula == null)
                return Result.Failure("Matrícula não encontrada");

            matricula.Trancar(request.Motivo);

            await _matriculaRepository.UpdateAsync(matricula, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao trancar matrícula: {ex.Message}");
        }
    }
}