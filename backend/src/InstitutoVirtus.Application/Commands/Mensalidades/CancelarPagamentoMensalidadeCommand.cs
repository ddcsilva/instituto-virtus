using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Commands.Mensalidades;

public class CancelarPagamentoMensalidadeCommand : IRequest<Result>
{
    public Guid MensalidadeId { get; set; }
}

public class CancelarPagamentoMensalidadeCommandHandler : IRequestHandler<CancelarPagamentoMensalidadeCommand, Result>
{
    private readonly IMensalidadeRepository _mensalidadeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelarPagamentoMensalidadeCommandHandler(
        IMensalidadeRepository mensalidadeRepository,
        IUnitOfWork unitOfWork)
    {
        _mensalidadeRepository = mensalidadeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelarPagamentoMensalidadeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var mensalidade = await _mensalidadeRepository.GetByIdAsync(request.MensalidadeId, cancellationToken);

            if (mensalidade == null)
                return Result.Failure("Mensalidade não encontrada");

            mensalidade.CancelarPagamento();

            await _mensalidadeRepository.UpdateAsync(mensalidade, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao cancelar pagamento: {ex.Message}");
        }
    }
}