using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Pessoa;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Turmas;

public class ObterAlunosDaTurmaQuery : IRequest<Result<List<AlunoResumoDto>>>
{
    public Guid TurmaId { get; set; }
}

public class ObterAlunosDaTurmaQueryHandler : IRequestHandler<ObterAlunosDaTurmaQuery, Result<List<AlunoResumoDto>>>
{
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IMapper _mapper;

    public ObterAlunosDaTurmaQueryHandler(
        IMatriculaRepository matriculaRepository,
        IMapper mapper)
    {
        _matriculaRepository = matriculaRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<AlunoResumoDto>>> Handle(ObterAlunosDaTurmaQuery request, CancellationToken cancellationToken)
    {
        var matriculas = await _matriculaRepository.GetByTurmaAsync(request.TurmaId, cancellationToken);
        var alunosAtivos = matriculas
            .Where(m => m.Status == StatusMatricula.Ativa)
            .Select(m => m.Aluno)
            .Where(a => a != null);

        var dto = _mapper.Map<List<AlunoResumoDto>>(alunosAtivos);
        return Result<List<AlunoResumoDto>>.Success(dto);
    }
}
