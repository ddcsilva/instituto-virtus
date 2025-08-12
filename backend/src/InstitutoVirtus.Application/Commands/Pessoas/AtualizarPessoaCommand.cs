namespace InstitutoVirtus.Application.Commands.Pessoas;

using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Pessoa;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Domain.ValueObjects;
using MediatR;

public class AtualizarPessoaCommand : IRequest<Result<PessoaDto>>
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime DataNascimento { get; set; }
    public string? Observacoes { get; set; }
    public bool Ativo { get; set; } = true;
}

public class AtualizarPessoaCommandHandler : IRequestHandler<AtualizarPessoaCommand, Result<PessoaDto>>
{
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AtualizarPessoaCommandHandler(IPessoaRepository pessoaRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _pessoaRepository = pessoaRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PessoaDto>> Handle(AtualizarPessoaCommand request, CancellationToken cancellationToken)
    {
        var pessoa = await _pessoaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (pessoa == null)
            return Result<PessoaDto>.Failure("Pessoa não encontrada");

        var telefone = new Telefone(request.Telefone);
        Email? email = string.IsNullOrWhiteSpace(request.Email) ? null : new Email(request.Email);
        Cpf? cpf = string.IsNullOrWhiteSpace(request.Cpf) ? null : new Cpf(request.Cpf);

        pessoa.AtualizarDadosComCpfData(
            request.NomeCompleto,
            cpf,
            telefone,
            email,
            request.DataNascimento,
            request.Observacoes,
            request.Ativo);

        await _pessoaRepository.UpdateAsync(pessoa, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<PessoaDto>(pessoa);
        return Result<PessoaDto>.Success(dto);
    }
}


