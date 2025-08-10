using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Presencas;

public class ObterRelatorioFrequenciaQuery : IRequest<Result<RelatorioFrequenciaDto>>
{
    public Guid TurmaId { get; set; }
}

public class RelatorioFrequenciaDto
{
    public string TurmaNome { get; set; } = string.Empty;
    public int TotalAulas { get; set; }
    public List<FrequenciaAlunoDto> Alunos { get; set; } = new();
}

public class FrequenciaAlunoDto
{
    public Guid AlunoId { get; set; }
    public string AlunoNome { get; set; } = string.Empty;
    public int TotalPresencas { get; set; }
    public int TotalFaltas { get; set; }
    public int TotalJustificadas { get; set; }
    public double PercentualFrequencia { get; set; }
}

public class ObterRelatorioFrequenciaQueryHandler : IRequestHandler<ObterRelatorioFrequenciaQuery, Result<RelatorioFrequenciaDto>>
{
    private readonly IAulaRepository _aulaRepository;
    private readonly ITurmaRepository _turmaRepository;
    private readonly IMatriculaRepository _matriculaRepository;

    public ObterRelatorioFrequenciaQueryHandler(
        IAulaRepository aulaRepository,
        ITurmaRepository turmaRepository,
        IMatriculaRepository matriculaRepository)
    {
        _aulaRepository = aulaRepository;
        _turmaRepository = turmaRepository;
        _matriculaRepository = matriculaRepository;
    }

    public async Task<Result<RelatorioFrequenciaDto>> Handle(ObterRelatorioFrequenciaQuery request, CancellationToken cancellationToken)
    {
        var turma = await _turmaRepository.GetByIdAsync(request.TurmaId, cancellationToken);
        if (turma == null)
            return Result<RelatorioFrequenciaDto>.Failure("Turma não encontrada");

        var aulas = await _aulaRepository.GetByTurmaAsync(request.TurmaId, cancellationToken);
        var matriculas = await _matriculaRepository.GetByTurmaAsync(request.TurmaId, cancellationToken);

        var relatorio = new RelatorioFrequenciaDto
        {
            TurmaNome = turma.ObterNome(),
            TotalAulas = aulas.Count(a => a.Realizada)
        };

        foreach (var matricula in matriculas.Where(m => m.Status == StatusMatricula.Ativa))
        {
            var presencas = aulas
                .SelectMany(a => a.Presencas)
                .Where(p => p.AlunoId == matricula.AlunoId)
                .ToList();

            var frequenciaAluno = new FrequenciaAlunoDto
            {
                AlunoId = matricula.AlunoId,
                AlunoNome = matricula.Aluno?.NomeCompleto ?? "",
                TotalPresencas = presencas.Count(p => p.Status == StatusPresenca.Presente),
                TotalFaltas = presencas.Count(p => p.Status == StatusPresenca.Falta),
                TotalJustificadas = presencas.Count(p => p.Status == StatusPresenca.Justificada)
            };

            var totalPresencasValidas = frequenciaAluno.TotalPresencas + frequenciaAluno.TotalJustificadas;
            frequenciaAluno.PercentualFrequencia = relatorio.TotalAulas > 0
                ? (double)totalPresencasValidas / relatorio.TotalAulas * 100
                : 100;

            relatorio.Alunos.Add(frequenciaAluno);
        }

        return Result<RelatorioFrequenciaDto>.Success(relatorio);
    }
}