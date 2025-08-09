using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Pagamento;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Domain.ValueObjects;
using MediatR;

namespace InstitutoVirtus.Application.Commands.Pagamentos;

public class RegistrarPagamentoCommand : IRequest<Result<PagamentoDto>>
{
    public Guid ResponsavelId { get; set; }
    public decimal ValorTotal { get; set; }
    public string MeioPagamento { get; set; } = string.Empty;
    public string? ComprovanteUrl { get; set; }
    public List<AlocacaoMensalidadeDto> Alocacoes { get; set; } = new();
}

public class AlocacaoMensalidadeDto
{
    public Guid MensalidadeId { get; set; }
    public decimal ValorAlocado { get; set; }
}

public class RegistrarPagamentoCommandHandler : IRequestHandler<RegistrarPagamentoCommand, Result<PagamentoDto>>
{
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly IMensalidadeRepository _mensalidadeRepository;
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegistrarPagamentoCommandHandler(
        IPagamentoRepository pagamentoRepository,
        IMensalidadeRepository mensalidadeRepository,
        IPessoaRepository pessoaRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _pagamentoRepository = pagamentoRepository;
        _mensalidadeRepository = mensalidadeRepository;
        _pessoaRepository = pessoaRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PagamentoDto>> Handle(RegistrarPagamentoCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Validar responsável
            var responsavel = await _pessoaRepository.GetByIdAsync(request.ResponsavelId, cancellationToken);
            if (responsavel == null || responsavel.TipoPessoa != TipoPessoa.Responsavel)
                return Result<PagamentoDto>.Failure("Responsável não encontrado");

            // Criar pagamento
            var meioPagamento = Enum.Parse<MeioPagamento>(request.MeioPagamento);
            var pagamento = new Pagamento(
                request.ResponsavelId,
                new Dinheiro(request.ValorTotal),
                meioPagamento,
                comprovanteUrl: request.ComprovanteUrl);

            // Alocar para mensalidades
            foreach (var alocacao in request.Alocacoes)
            {
                var mensalidade = await _mensalidadeRepository.GetByIdAsync(alocacao.MensalidadeId, cancellationToken);
                if (mensalidade == null)
                    continue;

                pagamento.AlocarParaMensalidade(alocacao.MensalidadeId, new Dinheiro(alocacao.ValorAlocado));
                mensalidade.RegistrarPagamento(meioPagamento);

                await _mensalidadeRepository.UpdateAsync(mensalidade, cancellationToken);
            }

            // Adicionar crédito se houver sobra
            var valorDisponivel = pagamento.ObterValorDisponivel();
            if (valorDisponivel.Valor > 0)
            {
                ((Responsavel)responsavel).AdicionarCredito(valorDisponivel.Valor);
                await _pessoaRepository.UpdateAsync(responsavel, cancellationToken);
            }

            await _pagamentoRepository.AddAsync(pagamento, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            var dto = _mapper.Map<PagamentoDto>(pagamento);
            return Result<PagamentoDto>.Success(dto);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<PagamentoDto>.Failure($"Erro ao registrar pagamento: {ex.Message}");
        }
    }
}
