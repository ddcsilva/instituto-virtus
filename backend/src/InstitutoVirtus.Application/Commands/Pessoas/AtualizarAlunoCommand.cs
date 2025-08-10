namespace InstitutoVirtus.Application.Commands.Pessoas;

using MediatR;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Pessoa;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Domain.Interfaces;
using AutoMapper;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.ValueObjects;

public class AtualizarAlunoCommand : IRequest<Result<AlunoDto>>
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Observacoes { get; set; }
}

public class AtualizarAlunoCommandHandler : IRequestHandler<AtualizarAlunoCommand, Result<AlunoDto>>
{
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AtualizarAlunoCommandHandler(
        IPessoaRepository pessoaRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _pessoaRepository = pessoaRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AlunoDto>> Handle(AtualizarAlunoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var aluno = await _pessoaRepository.GetByIdAsync(request.Id, cancellationToken);

            if (aluno == null || aluno.TipoPessoa != TipoPessoa.Aluno)
                return Result<AlunoDto>.Failure("Aluno não encontrado");

            var telefone = new Telefone(request.Telefone);
            Email? email = request.Email != null ? new Email(request.Email) : null;

            aluno.AtualizarDados(request.NomeCompleto, telefone, email, request.Observacoes);

            await _pessoaRepository.UpdateAsync(aluno, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<AlunoDto>(aluno);
            return Result<AlunoDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<AlunoDto>.Failure($"Erro ao atualizar aluno: {ex.Message}");
        }
    }
}