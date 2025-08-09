using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Nota;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Commands.Notas;

public class LancarNotasCommand : IRequest<Result>
{
    public Guid AvaliacaoId { get; set; }
    public List<LancarNotaDto> Notas { get; set; } = new();
}

public class LancarNotasCommandHandler : IRequestHandler<LancarNotasCommand, Result>
{
    private readonly IAvaliacaoRepository _avaliacaoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LancarNotasCommandHandler(
        IAvaliacaoRepository avaliacaoRepository,
        IUnitOfWork unitOfWork)
    {
        _avaliacaoRepository = avaliacaoRepository;
        _unitOfWork = unitOfWork;
  }

  public async Task<Result> Handle(LancarNotasCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var avaliacao = await _avaliacaoRepository.GetByIdAsync(request.AvaliacaoId, cancellationToken);
            if (avaliacao == null)
                return Result.Failure("Avaliação não encontrada");

            foreach (var nota in request.Notas)
            {
                avaliacao.LancarNota(nota.AlunoId, nota.Valor);
            }

            await _avaliacaoRepository.UpdateAsync(avaliacao, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao lançar notas: {ex.Message}");
        }
    }
}
