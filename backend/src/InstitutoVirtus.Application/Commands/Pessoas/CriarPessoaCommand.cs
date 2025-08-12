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

public class CriarPessoaCommand : IRequest<Result<PessoaDto>>
{
    public string NomeCompleto { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime DataNascimento { get; set; }
    public string TipoPessoa { get; set; } = string.Empty; // Aluno/Responsavel/Professor
    public string? Observacoes { get; set; }
}

public class CriarPessoaCommandHandler : IRequestHandler<CriarPessoaCommand, Result<PessoaDto>>
{
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CriarPessoaCommandHandler(IPessoaRepository pessoaRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _pessoaRepository = pessoaRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PessoaDto>> Handle(CriarPessoaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Unicidade básica por telefone
            if (await _pessoaRepository.ExistsByTelefoneAsync(request.Telefone, cancellationToken))
                return Result<PessoaDto>.Failure("Telefone já cadastrado");

            var telefone = new Telefone(request.Telefone);
            Email? email = string.IsNullOrWhiteSpace(request.Email) ? null : new Email(request.Email);
            Cpf? cpf = string.IsNullOrWhiteSpace(request.Cpf) ? null : new Cpf(request.Cpf);

            Enum.TryParse<TipoPessoa>(request.TipoPessoa, out var tipo);

            Pessoa pessoa = tipo switch
            {
                TipoPessoa.Aluno => new Aluno(request.NomeCompleto, cpf, telefone, email, request.DataNascimento, request.Observacoes),
                TipoPessoa.Professor => new Professor(request.NomeCompleto, cpf, telefone, email!, request.DataNascimento, null, request.Observacoes),
                TipoPessoa.Responsavel => new Responsavel(request.NomeCompleto, cpf, telefone, email!, request.DataNascimento, request.Observacoes),
                _ => new Pessoa(request.NomeCompleto, cpf, telefone, email, request.DataNascimento, TipoPessoa.Coordenador, request.Observacoes)
            };

            await _pessoaRepository.AddAsync(pessoa, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<PessoaDto>(pessoa);
            return Result<PessoaDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<PessoaDto>.Failure($"Erro ao criar pessoa: {ex.Message}");
        }
    }
}


