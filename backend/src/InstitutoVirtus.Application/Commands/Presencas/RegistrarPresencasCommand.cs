using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Presenca;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Commands.Presencas;

public class RegistrarPresencasCommand : IRequest<Result>
{
    public Guid AulaId { get; set; }
    public List<RegistrarPresencaDto> Presencas { get; set; } = new();
}

public class RegistrarPresencasCommandHandler : IRequestHandler<RegistrarPresencasCommand, Result>
{
    private readonly IAulaRepository _aulaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarPresencasCommandHandler(
        IAulaRepository aulaRepository,
        IUnitOfWork unitOfWork)
    {
        _aulaRepository = aulaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RegistrarPresencasCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var aula = await _aulaRepository.GetByIdAsync(request.AulaId, cancellationToken);
            if (aula == null)
                return Result.Failure("Aula não encontrada");

            foreach (var presenca in request.Presencas)
            {
                var status = Enum.Parse<StatusPresenca>(presenca.Status);
                aula.RegistrarPresenca(presenca.AlunoId, status, presenca.Justificativa);
            }

            aula.MarcarComoRealizada();

            // Como o aggregate 'aula' está sendo rastreado pelo contexto, basta salvar
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao registrar presenças: {ex.Message}");
        }
    }
}
