using CsvHelper;
using CsvHelper.Configuration;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace InstitutoVirtus.Application.Commands.Pagamentos;

public class ProcessarArquivoPagamentoCommand : IRequest<Result<ProcessarArquivoResult>>
{
    public Stream Arquivo { get; set; } = null!;
    public string NomeArquivo { get; set; } = string.Empty;
    public string Formato { get; set; } = "csv"; // csv ou excel
}

public class ProcessarArquivoResult
{
    public int TotalProcessados { get; set; }
    public int TotalSucesso { get; set; }
    public int TotalErros { get; set; }
    public List<string> Erros { get; set; } = new();
    public List<PagamentoProcessado> Pagamentos { get; set; } = new();
}

public class PagamentoProcessado
{
    public string ResponsavelEmail { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataPagamento { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Erro { get; set; }
}

// Classe para mapear o CSV
public class PagamentoCsvRecord
{
    public string ResponsavelEmail { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public string DataPagamento { get; set; } = string.Empty;
    public string MeioPagamento { get; set; } = string.Empty;
    public string? ReferenciaExterna { get; set; }
    public string? AlunosIds { get; set; } // IDs dos alunos separados por vírgula
}

public class PagamentoCsvMap : ClassMap<PagamentoCsvRecord>
{
    public PagamentoCsvMap()
    {
        Map(m => m.ResponsavelEmail).Name("Email");
        Map(m => m.Valor).Name("Valor");
        Map(m => m.DataPagamento).Name("Data");
        Map(m => m.MeioPagamento).Name("Forma");
        Map(m => m.ReferenciaExterna).Name("Referencia").Optional();
        Map(m => m.AlunosIds).Name("Alunos").Optional();
    }
}

public class ProcessarArquivoPagamentoCommandHandler : IRequestHandler<ProcessarArquivoPagamentoCommand, Result<ProcessarArquivoResult>>
{
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly IMensalidadeRepository _mensalidadeRepository;
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessarArquivoPagamentoCommandHandler> _logger;

    public ProcessarArquivoPagamentoCommandHandler(
        IPagamentoRepository pagamentoRepository,
        IMensalidadeRepository mensalidadeRepository,
        IPessoaRepository pessoaRepository,
        IUnitOfWork unitOfWork,
        ILogger<ProcessarArquivoPagamentoCommandHandler> logger)
    {
        _pagamentoRepository = pagamentoRepository;
        _mensalidadeRepository = mensalidadeRepository;
        _pessoaRepository = pessoaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ProcessarArquivoResult>> Handle(ProcessarArquivoPagamentoCommand request, CancellationToken cancellationToken)
    {
        var result = new ProcessarArquivoResult();

        try
        {
            List<PagamentoCsvRecord> records;

            if (request.Formato.ToLower() == "excel")
            {
                records = await ProcessarExcel(request.Arquivo);
            }
            else
            {
                records = await ProcessarCsv(request.Arquivo);
            }

            foreach (var record in records)
            {
                result.TotalProcessados++;

                var processamento = await ProcessarPagamento(record, cancellationToken);
                result.Pagamentos.Add(processamento);

                if (processamento.Status == "Sucesso")
                {
                    result.TotalSucesso++;
                }
                else
                {
                    result.TotalErros++;
                    result.Erros.Add($"Linha {result.TotalProcessados}: {processamento.Erro}");
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Arquivo processado: {result.TotalSucesso} sucessos, {result.TotalErros} erros");

            return Result<ProcessarArquivoResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar arquivo de pagamentos");
            return Result<ProcessarArquivoResult>.Failure($"Erro ao processar arquivo: {ex.Message}");
        }
    }

    private async Task<List<PagamentoCsvRecord>> ProcessarCsv(Stream arquivo)
    {
        using var reader = new StreamReader(arquivo);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
            BadDataFound = null
        });

        csv.Context.RegisterClassMap<PagamentoCsvMap>();
        var records = csv.GetRecords<PagamentoCsvRecord>().ToList();
        return await Task.FromResult(records);
    }

    private async Task<List<PagamentoCsvRecord>> ProcessarExcel(Stream arquivo)
    {
        // Implementar com ClosedXML ou EPPlus
        // Por enquanto, retornar lista vazia
        _logger.LogWarning("Processamento de Excel ainda não implementado");
        return await Task.FromResult(new List<PagamentoCsvRecord>());
    }

    private async Task<PagamentoProcessado> ProcessarPagamento(PagamentoCsvRecord record, CancellationToken cancellationToken)
    {
        var processado = new PagamentoProcessado
        {
            ResponsavelEmail = record.ResponsavelEmail,
            Status = "Erro"
        };

        try
        {
            // Validar e converter dados
            if (!decimal.TryParse(record.Valor.Replace("R$", "").Replace(",", ".").Trim(), out var valor))
            {
                processado.Erro = "Valor inválido";
                return processado;
            }
            processado.Valor = valor;

            if (!DateTime.TryParse(record.DataPagamento, out var dataPagamento))
            {
                processado.Erro = "Data inválida";
                return processado;
            }
            processado.DataPagamento = dataPagamento;

            // Buscar responsável
            var responsavel = await _pessoaRepository.GetByEmailAsync(record.ResponsavelEmail, cancellationToken);
            if (responsavel == null || responsavel.TipoPessoa != TipoPessoa.Responsavel)
            {
                processado.Erro = "Responsável não encontrado";
                return processado;
            }

            // Verificar se pagamento já existe (evitar duplicação)
            if (!string.IsNullOrEmpty(record.ReferenciaExterna))
            {
                var pagamentoExistente = await _pagamentoRepository.GetByReferenciaExternaAsync(record.ReferenciaExterna, cancellationToken);
                if (pagamentoExistente != null)
                {
                    processado.Erro = "Pagamento já processado";
                    return processado;
                }
            }

            // Determinar meio de pagamento
            var meioPagamento = DeterminarMeioPagamento(record.MeioPagamento);

            // Criar pagamento
            var pagamento = new Pagamento(
                responsavel.Id,
                new Dinheiro(valor),
                meioPagamento,
                dataPagamento,
                record.ReferenciaExterna
            );

            // Buscar mensalidades em aberto do responsável
            var mensalidades = await _mensalidadeRepository.GetEmAbertoByResponsavelAsync(responsavel.Id, cancellationToken);

            // Se foram especificados alunos, filtrar mensalidades
            if (!string.IsNullOrEmpty(record.AlunosIds))
            {
                var alunosIds = record.AlunosIds.Split(',').Select(id => Guid.Parse(id.Trim())).ToList();
                mensalidades = mensalidades.Where(m => alunosIds.Contains(m.Matricula.AlunoId));
            }

            // Alocar pagamento para mensalidades (dos mais antigos primeiro)
            var valorRestante = valor;
            foreach (var mensalidade in mensalidades.OrderBy(m => m.DataVencimento))
            {
                if (valorRestante <= 0) break;

                var valorAlocar = Math.Min(valorRestante, mensalidade.Valor.Valor);
                pagamento.AlocarParaMensalidade(mensalidade.Id, new Dinheiro(valorAlocar));
                mensalidade.RegistrarPagamento(meioPagamento, dataPagamento);

                await _mensalidadeRepository.UpdateAsync(mensalidade, cancellationToken);
                valorRestante -= valorAlocar;
            }

            // Se sobrou valor, adicionar como crédito
            if (valorRestante > 0)
            {
                ((Responsavel)responsavel).AdicionarCredito(valorRestante);
                await _pessoaRepository.UpdateAsync(responsavel, cancellationToken);
            }

            await _pagamentoRepository.AddAsync(pagamento, cancellationToken);

            processado.Status = "Sucesso";
            processado.Erro = null;
        }
        catch (Exception ex)
        {
            processado.Erro = ex.Message;
            _logger.LogError(ex, $"Erro ao processar pagamento: {record.ResponsavelEmail}");
        }

        return processado;
    }

    private MeioPagamento DeterminarMeioPagamento(string meio)
    {
        return meio.ToLower() switch
        {
            "pix" => MeioPagamento.Pix,
            "dinheiro" => MeioPagamento.Dinheiro,
            "boleto" => MeioPagamento.Boleto,
            "cartao" or "cartão" => MeioPagamento.CartaoCredito,
            "debito" or "débito" => MeioPagamento.CartaoDebito,
            "transferencia" or "transferência" => MeioPagamento.Transferencia,
            _ => MeioPagamento.Outro
        };
    }
}
