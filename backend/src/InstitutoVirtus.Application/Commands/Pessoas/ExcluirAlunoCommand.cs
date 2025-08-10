using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Commands.Pessoas;

public class ExcluirAlunoCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

public class ExcluirAlunoCommandHandler : IRequestHandler<ExcluirAlunoCommand, Result>
{
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ExcluirAlunoCommandHandler(
        IPessoaRepository pessoaRepository,
        IMatriculaRepository matriculaRepository,
        IUnitOfWork unitOfWork)
    {
        _pessoaRepository = pessoaRepository;
        _matriculaRepository = matriculaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ExcluirAlunoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var aluno = await _pessoaRepository.GetByIdAsync(request.Id, cancellationToken);

            if (aluno == null || aluno.TipoPessoa != TipoPessoa.Aluno)
                return Result.Failure("Aluno não encontrado");

            // Verificar se tem matrículas ativas
            var matriculasAtivas = await _matriculaRepository.GetByAlunoAsync(request.Id, cancellationToken);
            if (matriculasAtivas.Any(m => m.Status == StatusMatricula.Ativa))
                return Result.Failure("Não é possível excluir aluno com matrículas ativas");

            // Desativar ao invés de excluir fisicamente
            aluno.Desativar();
            await _pessoaRepository.UpdateAsync(aluno, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao excluir aluno: {ex.Message}");
        }
    }
}