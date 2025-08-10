using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Commands.Pagamentos;

public class ProcessarArquivoPagamentoCommand : IRequest<Result<ProcessarArquivoResult>>
{
    public Stream Arquivo { get; set; } = null!;
    public string NomeArquivo { get; set; } = string.Empty;
}

public class ProcessarArquivoResult
{
    public int TotalProcessados { get; set; }
    public int TotalSucesso { get; set; }
    public int TotalErros { get; set; }
    public List<string> Erros { get; set; } = new();
}

public class ProcessarArquivoPagamentoCommandHandler : IRequestHandler<ProcessarArquivoPagamentoCommand, Result<ProcessarArquivoResult>>
{
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly IMensalidadeRepository _mensalidadeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessarArquivoPagamentoCommandHandler(
        IPagamentoRepository pagamentoRepository,
        IMensalidadeRepository mensalidadeRepository,
        IUnitOfWork unitOfWork)
    {
        _pagamentoRepository = pagamentoRepository;
        _mensalidadeRepository = mensalidadeRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<Result<ProcessarArquivoResult>> Handle(ProcessarArquivoPagamentoCommand request, CancellationToken cancellationToken)
    {
        var result = new ProcessarArquivoResult();

        try
        {
            // TODO: Implementar leitura do arquivo (CSV, Excel, etc.)
            // Por enquanto, retorno simulado

            result.TotalProcessados = 10;
            result.TotalSucesso = 8;
            result.TotalErros = 2;
            result.Erros.Add("Linha 5: Valor inválido");
            result.Erros.Add("Linha 8: Aluno não encontrado");

            return Task.FromResult(Result<ProcessarArquivoResult>.Success(result));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<ProcessarArquivoResult>.Failure($"Erro ao processar arquivo: {ex.Message}"));
        }
    }
}
