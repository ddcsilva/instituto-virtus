using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Relatorios;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Relatorios;

public class ObterBoletimQuery : IRequest<Result<BoletimDto>>
{
    public Guid AlunoId { get; set; }
    public Guid TurmaId { get; set; }
}

public class ObterBoletimQueryHandler : IRequestHandler<ObterBoletimQuery, Result<BoletimDto>>
{
    private readonly IAvaliacaoRepository _avaliacaoRepository;
    private readonly IPessoaRepository _pessoaRepository;
    private readonly ITurmaRepository _turmaRepository;

    public ObterBoletimQueryHandler(
        IAvaliacaoRepository avaliacaoRepository,
        IPessoaRepository pessoaRepository,
        ITurmaRepository turmaRepository)
    {
        _avaliacaoRepository = avaliacaoRepository;
        _pessoaRepository = pessoaRepository;
        _turmaRepository = turmaRepository;
    }

    public async Task<Result<BoletimDto>> Handle(ObterBoletimQuery request, CancellationToken cancellationToken)
    {
        var aluno = await _pessoaRepository.GetByIdAsync(request.AlunoId, cancellationToken);
        if (aluno == null)
            return Result<BoletimDto>.Failure("Aluno não encontrado");

        var turma = await _turmaRepository.GetByIdAsync(request.TurmaId, cancellationToken);
        if (turma == null)
            return Result<BoletimDto>.Failure("Turma não encontrada");

        var mediaFinal = await _avaliacaoRepository.CalcularMediaFinalAsync(request.AlunoId, request.TurmaId, cancellationToken);
        var frequencia = await _avaliacaoRepository.CalcularFrequenciaAsync(request.AlunoId, request.TurmaId, cancellationToken);

        var aprovado = mediaFinal >= 6.0m && frequencia >= 75.0;

        var boletim = new BoletimDto
        {
            AlunoId = request.AlunoId,
            AlunoNome = aluno.NomeCompleto,
            TurmaId = request.TurmaId,
            TurmaNome = turma.ObterNome(),
            MediaFinal = mediaFinal,
            Frequencia = frequencia,
            Aprovado = aprovado,
            Situacao = aprovado ? "Aprovado" : "Reprovado"
        };

        // Buscar notas detalhadas
        var avaliacoes = await _avaliacaoRepository.GetByTurmaAsync(request.TurmaId, cancellationToken);
        foreach (var avaliacao in avaliacoes)
        {
            var nota = avaliacao.Notas.FirstOrDefault(n => n.AlunoId == request.AlunoId);
            if (nota != null)
            {
                boletim.Notas.Add(new NotaBoletimDto
                {
                    Avaliacao = avaliacao.Nome,
                    Nota = nota.Valor,
                    Peso = avaliacao.Peso
                });
            }
        }

        return Result<BoletimDto>.Success(boletim);
    }
}
