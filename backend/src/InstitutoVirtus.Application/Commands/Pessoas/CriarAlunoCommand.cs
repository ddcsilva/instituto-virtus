namespace InstitutoVirtus.Application.Commands.Pessoas;

using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Pessoa;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Domain.ValueObjects;
using MediatR;

public class CriarAlunoCommand : IRequest<Result<AlunoDto>>
{
    public string NomeCompleto { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime DataNascimento { get; set; }
    public string? Observacoes { get; set; }
    public Guid? ResponsavelId { get; set; }
    public string? Parentesco { get; set; }
}

public class CriarAlunoCommandHandler : IRequestHandler<CriarAlunoCommand, Result<AlunoDto>>
{
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CriarAlunoCommandHandler(
        IPessoaRepository pessoaRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _pessoaRepository = pessoaRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AlunoDto>> Handle(CriarAlunoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validar telefone único
            if (await _pessoaRepository.ExistsByTelefoneAsync(request.Telefone, cancellationToken))
                return Result<AlunoDto>.Failure("Telefone já cadastrado");

            // Criar aluno
            var telefone = new Telefone(request.Telefone);
            Cpf? cpf = !string.IsNullOrWhiteSpace(request.Cpf) ? new Cpf(request.Cpf) : null;
            Email? email = request.Email != null ? new Email(request.Email) : null;

            var aluno = new Aluno(
                request.NomeCompleto,
                cpf,
                telefone,
                email,
                request.DataNascimento,
                request.Observacoes);

            // Vincular responsável se fornecido
            if (request.ResponsavelId.HasValue && aluno.EhMenorDeIdade())
            {
                var responsavel = await _pessoaRepository.GetByIdAsync(request.ResponsavelId.Value, cancellationToken);
                if (responsavel == null || responsavel.TipoPessoa != TipoPessoa.Responsavel)
                    return Result<AlunoDto>.Failure("Responsável não encontrado");

                var parentesco = Enum.Parse<Parentesco>(request.Parentesco ?? "Responsavel");
                aluno.AdicionarResponsavel((Responsavel)responsavel, parentesco);
            }

            await _pessoaRepository.AddAsync(aluno, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<AlunoDto>(aluno);
            return Result<AlunoDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<AlunoDto>.Failure($"Erro ao criar aluno: {ex.Message}");
        }
    }
}
