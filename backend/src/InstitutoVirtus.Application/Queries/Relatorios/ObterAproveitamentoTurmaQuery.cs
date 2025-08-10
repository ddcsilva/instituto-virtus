using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Relatorios;

public class ObterAproveitamentoTurmaQuery : IRequest<Result<AproveitamentoTurmaDto>>
{
    public Guid TurmaId { get; set; }
}

public class AproveitamentoTurmaDto
{
    public string TurmaNome { get; set; } = string.Empty;
    public int TotalAlunos { get; set; }
    public int Aprovados { get; set; }
    public int Reprovados { get; set; }
    public double MediaGeral { get; set; }
    public double FrequenciaMedia { get; set; }
    public List<AproveitamentoAlunoDto> Alunos { get; set; } = new();
}

public class AproveitamentoAlunoDto
{
    public Guid AlunoId { get; set; }
    public string AlunoNome { get; set; } = string.Empty;
    public decimal MediaFinal { get; set; }
    public double Frequencia { get; set; }
    public bool Aprovado { get; set; }
    public string Situacao { get; set; } = string.Empty;
}

public class ObterAproveitamentoTurmaQueryHandler : IRequestHandler<ObterAproveitamentoTurmaQuery, Result<AproveitamentoTurmaDto>>
{
    private readonly ITurmaRepository _turmaRepository;
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IAvaliacaoRepository _avaliacaoRepository;

    public ObterAproveitamentoTurmaQueryHandler(
        ITurmaRepository turmaRepository,
        IMatriculaRepository matriculaRepository,
        IAvaliacaoRepository avaliacaoRepository)
    {
        _turmaRepository = turmaRepository;
        _matriculaRepository = matriculaRepository;
        _avaliacaoRepository = avaliacaoRepository;
    }

    public async Task<Result<AproveitamentoTurmaDto>> Handle(ObterAproveitamentoTurmaQuery request, CancellationToken cancellationToken)
    {
        var turma = await _turmaRepository.GetByIdAsync(request.TurmaId, cancellationToken);
        if (turma == null)
            return Result<AproveitamentoTurmaDto>.Failure("Turma não encontrada");

        var matriculas = await _matriculaRepository.GetByTurmaAsync(request.TurmaId, cancellationToken);
        var matriculasAtivas = matriculas.Where(m => m.Status == StatusMatricula.Ativa).ToList();

        var aproveitamento = new AproveitamentoTurmaDto
        {
            TurmaNome = turma.ObterNome(),
            TotalAlunos = matriculasAtivas.Count
        };

        foreach (var matricula in matriculasAtivas)
        {
            var mediaFinal = await _avaliacaoRepository.CalcularMediaFinalAsync(matricula.AlunoId, request.TurmaId, cancellationToken);
            var frequencia = await _avaliacaoRepository.CalcularFrequenciaAsync(matricula.AlunoId, request.TurmaId, cancellationToken);

            var aprovado = mediaFinal >= 6.0m && frequencia >= 75.0;

            var aproveitamentoAluno = new AproveitamentoAlunoDto
            {
                AlunoId = matricula.AlunoId,
                AlunoNome = matricula.Aluno?.NomeCompleto ?? "",
                MediaFinal = mediaFinal,
                Frequencia = frequencia,
                Aprovado = aprovado,
                Situacao = aprovado ? "Aprovado" :
                    (mediaFinal < 6.0m ? "Reprovado por Nota" : "Reprovado por Frequência")
            };

            aproveitamento.Alunos.Add(aproveitamentoAluno);

            if (aprovado)
                aproveitamento.Aprovados++;
            else
                aproveitamento.Reprovados++;
        }

        aproveitamento.MediaGeral = aproveitamento.Alunos.Any()
            ? (double)aproveitamento.Alunos.Average(a => a.MediaFinal)
            : 0;

        aproveitamento.FrequenciaMedia = aproveitamento.Alunos.Any()
            ? aproveitamento.Alunos.Average(a => a.Frequencia)
            : 0;

        return Result<AproveitamentoTurmaDto>.Success(aproveitamento);
    }
}