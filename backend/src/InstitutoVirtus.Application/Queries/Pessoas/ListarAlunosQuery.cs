namespace InstitutoVirtus.Application.Queries.Pessoas;

using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Pessoa;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

public class ListarAlunosQuery : IRequest<Result<PagedResult<AlunoDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public bool? Ativo { get; set; }
}

public class ListarAlunosQueryHandler : IRequestHandler<ListarAlunosQuery, Result<PagedResult<AlunoDto>>>
{
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IMapper _mapper;

    public ListarAlunosQueryHandler(IPessoaRepository pessoaRepository, IMapper mapper)
    {
        _pessoaRepository = pessoaRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<AlunoDto>>> Handle(ListarAlunosQuery request, CancellationToken cancellationToken)
    {
        var alunos = await _pessoaRepository.GetByTipoAsync(TipoPessoa.Aluno, cancellationToken);

        // Aplicar filtros
        if (!string.IsNullOrEmpty(request.Search))
        {
            alunos = alunos.Where(a =>
                a.NomeCompleto.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                a.Telefone.Numero.Contains(request.Search) ||
                (a.Email != null && a.Email.Endereco.Contains(request.Search, StringComparison.OrdinalIgnoreCase)));
        }

        if (request.Ativo.HasValue)
        {
            alunos = alunos.Where(a => a.Ativo == request.Ativo.Value);
        }

        // Paginação
        var totalCount = alunos.Count();
        var items = alunos
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dto = _mapper.Map<List<AlunoDto>>(items);

        var result = new PagedResult<AlunoDto>(dto, totalCount, request.PageNumber, request.PageSize);
        return Result<PagedResult<AlunoDto>>.Success(result);
    }
}
