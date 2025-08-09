using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Pessoa;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Pessoas;

public class ObterAlunoQuery : IRequest<Result<AlunoDto>>
{
    public Guid Id { get; set; }
}

public class ObterAlunoQueryHandler : IRequestHandler<ObterAlunoQuery, Result<AlunoDto>>
{
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IMapper _mapper;

    public ObterAlunoQueryHandler(IPessoaRepository pessoaRepository, IMapper mapper)
    {
        _pessoaRepository = pessoaRepository;
        _mapper = mapper;
    }

    public async Task<Result<AlunoDto>> Handle(ObterAlunoQuery request, CancellationToken cancellationToken)
    {
        var aluno = await _pessoaRepository.GetByIdAsync(request.Id, cancellationToken);

        if (aluno == null || aluno.TipoPessoa != TipoPessoa.Aluno)
            return Result<AlunoDto>.Failure("Aluno não encontrado");

        var dto = _mapper.Map<AlunoDto>(aluno);
        return Result<AlunoDto>.Success(dto);
    }
}
