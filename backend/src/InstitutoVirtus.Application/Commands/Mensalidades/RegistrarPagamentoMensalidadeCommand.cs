using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Commands.Mensalidades;

public class RegistrarPagamentoMensalidadeCommand : IRequest<Result>
{
    public Guid MensalidadeId { get; set; }
    public string MeioPagamento { get; set; } = string.Empty;
    public DateTime? DataPagamento { get; set; }
    public string? Observacao { get; set; }
}

public class RegistrarPagamentoMensalidadeCommandHandler : IRequestHandler<RegistrarPagamentoMensalidadeCommand, Result>
{
    private readonly IMensalidadeRepository _mensalidadeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarPagamentoMensalidadeCommandHandler(
        IMensalidadeRepository mensalidadeRepository,
        IUnitOfWork unitOfWork)
    {
        _mensalidadeRepository = mensalidadeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RegistrarPagamentoMensalidadeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var mensalidade = await _mensalidadeRepository.GetByIdAsync(request.MensalidadeId, cancellationToken);

            if (mensalidade == null)
                return Result.Failure("Mensalidade não encontrada");

            var meioPagamento = Enum.Parse<MeioPagamento>(request.MeioPagamento);
            mensalidade.RegistrarPagamento(meioPagamento, request.DataPagamento, request.Observacao);

            await _mensalidadeRepository.UpdateAsync(mensalidade, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao registrar pagamento: {ex.Message}");
        }
    }
}