namespace InstitutoVirtus.Application.Commands.Pessoas;

using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Pessoa;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

public class TogglePessoaStatusCommand : IRequest<Result<PessoaDto>>
{
    public Guid Id { get; set; }
}

public class TogglePessoaStatusCommandHandler : IRequestHandler<TogglePessoaStatusCommand, Result<PessoaDto>>
{
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TogglePessoaStatusCommandHandler(IPessoaRepository pessoaRepository, IUnitOfWork unitOfWork)
    {
        _pessoaRepository = pessoaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PessoaDto>> Handle(TogglePessoaStatusCommand request, CancellationToken cancellationToken)
    {
        var pessoa = await _pessoaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (pessoa == null)
            return Result<PessoaDto>.Failure("Pessoa não encontrada");

        pessoa.AlternarStatus();
        await _pessoaRepository.UpdateAsync(pessoa, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PessoaDto>.Success(new PessoaDto
        {
            Id = pessoa.Id,
            NomeCompleto = pessoa.NomeCompleto,
            Cpf = pessoa.Cpf?.Numero,
            Telefone = pessoa.Telefone.NumeroFormatado(),
            Email = pessoa.Email?.Endereco,
            DataNascimento = pessoa.DataNascimento,
            TipoPessoa = pessoa.TipoPessoa.ToString(),
            Observacoes = pessoa.Observacoes,
            Ativo = pessoa.Ativo,
            Idade = pessoa.CalcularIdade()
        });
    }
}


