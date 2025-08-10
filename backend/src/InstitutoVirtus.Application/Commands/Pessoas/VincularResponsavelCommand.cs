using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Commands.Pessoas;

public class VincularResponsavelCommand : IRequest<Result>
{
    public Guid AlunoId { get; set; }
    public Guid ResponsavelId { get; set; }
    public string Parentesco { get; set; } = string.Empty;
    public bool Principal { get; set; }
}

public class VincularResponsavelCommandHandler : IRequestHandler<VincularResponsavelCommand, Result>
{
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VincularResponsavelCommandHandler(
        IPessoaRepository pessoaRepository,
        IUnitOfWork unitOfWork)
    {
        _pessoaRepository = pessoaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(VincularResponsavelCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var aluno = await _pessoaRepository.GetByIdAsync(request.AlunoId, cancellationToken);
            if (aluno == null || aluno.TipoPessoa != TipoPessoa.Aluno)
                return Result.Failure("Aluno não encontrado");

            var responsavel = await _pessoaRepository.GetByIdAsync(request.ResponsavelId, cancellationToken);
            if (responsavel == null || responsavel.TipoPessoa != TipoPessoa.Responsavel)
                return Result.Failure("Responsável não encontrado");

            var parentesco = Enum.Parse<Parentesco>(request.Parentesco);
            ((Aluno)aluno).AdicionarResponsavel((Responsavel)responsavel, parentesco);

            await _pessoaRepository.UpdateAsync(aluno, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao vincular responsável: {ex.Message}");
        }
    }
}