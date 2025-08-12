namespace InstitutoVirtus.Application.Queries.Pessoas;

using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Pessoa;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

public class ObterPessoaQuery : IRequest<Result<PessoaDto>>
{
    public Guid Id { get; set; }
}

public class ObterPessoaQueryHandler : IRequestHandler<ObterPessoaQuery, Result<PessoaDto>>
{
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IMapper _mapper;

    public ObterPessoaQueryHandler(IPessoaRepository pessoaRepository, IMapper mapper)
    {
        _pessoaRepository = pessoaRepository;
        _mapper = mapper;
    }

    public async Task<Result<PessoaDto>> Handle(ObterPessoaQuery request, CancellationToken cancellationToken)
    {
        var pessoa = await _pessoaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (pessoa == null)
        {
            return Result<PessoaDto>.Failure("Pessoa não encontrada");
        }

        var dto = _mapper.Map<PessoaDto>(pessoa);
        return Result<PessoaDto>.Success(dto);
    }
}


