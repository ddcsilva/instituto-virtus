using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Pessoa;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Pessoas;

public class ObterResponsaveisDoAlunoQuery : IRequest<Result<List<ResponsavelDto>>>
{
    public Guid AlunoId { get; set; }
}

public class ObterResponsaveisDoAlunoQueryHandler : IRequestHandler<ObterResponsaveisDoAlunoQuery, Result<List<ResponsavelDto>>>
{
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IMapper _mapper;

    public ObterResponsaveisDoAlunoQueryHandler(IPessoaRepository pessoaRepository, IMapper mapper)
    {
        _pessoaRepository = pessoaRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<ResponsavelDto>>> Handle(ObterResponsaveisDoAlunoQuery request, CancellationToken cancellationToken)
    {
        var aluno = await _pessoaRepository.GetByIdAsync(request.AlunoId, cancellationToken);

        if (aluno == null || aluno.TipoPessoa != TipoPessoa.Aluno)
            return Result<List<ResponsavelDto>>.Failure("Aluno não encontrado");

        var responsaveisIds = ((Aluno)aluno).Responsaveis.Select(r => r.ResponsavelId);
        var responsaveis = new List<Responsavel>();

        foreach (var id in responsaveisIds)
        {
            var responsavel = await _pessoaRepository.GetByIdAsync(id, cancellationToken);
            if (responsavel != null && responsavel.TipoPessoa == TipoPessoa.Responsavel)
                responsaveis.Add((Responsavel)responsavel);
        }

        var dto = _mapper.Map<List<ResponsavelDto>>(responsaveis);
        return Result<List<ResponsavelDto>>.Success(dto);
    }
}