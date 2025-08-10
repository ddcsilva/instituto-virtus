using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Avaliacao;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Commands.Avaliacoes;

public class CriarAvaliacaoCommand : IRequest<Result<AvaliacaoDto>>
{
    public Guid TurmaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Peso { get; set; } = 1;
    public DateTime? DataAplicacao { get; set; }
    public string? Descricao { get; set; }
}

public class CriarAvaliacaoCommandHandler : IRequestHandler<CriarAvaliacaoCommand, Result<AvaliacaoDto>>
{
    private readonly IAvaliacaoRepository _avaliacaoRepository;
    private readonly ITurmaRepository _turmaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CriarAvaliacaoCommandHandler(
        IAvaliacaoRepository avaliacaoRepository,
        ITurmaRepository turmaRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _avaliacaoRepository = avaliacaoRepository;
        _turmaRepository = turmaRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AvaliacaoDto>> Handle(CriarAvaliacaoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var turma = await _turmaRepository.GetByIdAsync(request.TurmaId, cancellationToken);
            if (turma == null)
                return Result<AvaliacaoDto>.Failure("Turma não encontrada");

            var avaliacao = new Avaliacao(
                request.TurmaId,
                request.Nome,
                request.Peso,
                request.DataAplicacao);

            await _avaliacaoRepository.AddAsync(avaliacao, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<AvaliacaoDto>(avaliacao);
            return Result<AvaliacaoDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<AvaliacaoDto>.Failure($"Erro ao criar avaliação: {ex.Message}");
        }
    }
}