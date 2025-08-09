using AutoMapper;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Matricula;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Commands.Matriculas;

public class RealizarMatriculaCommand : IRequest<Result<MatriculaDto>>
{
    public Guid AlunoId { get; set; }
    public Guid TurmaId { get; set; }
    public int MesesQuantidade { get; set; } = 12;
    public int DiaVencimento { get; set; } = 10;
}

public class RealizarMatriculaCommandHandler : IRequestHandler<RealizarMatriculaCommand, Result<MatriculaDto>>
{
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly ITurmaRepository _turmaRepository;
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RealizarMatriculaCommandHandler(
        IMatriculaRepository matriculaRepository,
        ITurmaRepository turmaRepository,
        IPessoaRepository pessoaRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _matriculaRepository = matriculaRepository;
        _turmaRepository = turmaRepository;
        _pessoaRepository = pessoaRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<MatriculaDto>> Handle(RealizarMatriculaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validar aluno
            var aluno = await _pessoaRepository.GetByIdAsync(request.AlunoId, cancellationToken);
            if (aluno == null || aluno.TipoPessoa != TipoPessoa.Aluno)
                return Result<MatriculaDto>.Failure("Aluno não encontrado");

            // Validar turma
            var turma = await _turmaRepository.GetByIdAsync(request.TurmaId, cancellationToken);
            if (turma == null)
                return Result<MatriculaDto>.Failure("Turma não encontrada");

            if (!turma.TemVaga())
                return Result<MatriculaDto>.Failure("Turma sem vagas disponíveis");

            // Verificar matrícula duplicada
            if (await _matriculaRepository.AlunoJaMatriculadoNaTurmaAsync(request.AlunoId, request.TurmaId, cancellationToken))
                return Result<MatriculaDto>.Failure("Aluno já matriculado nesta turma");

            // Criar matrícula
            var matricula = new Matricula(request.AlunoId, request.TurmaId);

            // Gerar mensalidades
            matricula.GerarMensalidades(
                request.MesesQuantidade,
                turma.Curso?.ValorMensalidade ?? 0,
                request.DiaVencimento);

            await _matriculaRepository.AddAsync(matricula, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<MatriculaDto>(matricula);
            return Result<MatriculaDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<MatriculaDto>.Failure($"Erro ao realizar matrícula: {ex.Message}");
        }
    }
}
