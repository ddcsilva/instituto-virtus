using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoVirtus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IPessoaRepository _pessoaRepository;
    private readonly ITurmaRepository _turmaRepository;
    private readonly IMensalidadeRepository _mensalidadeRepository;
    private readonly IAulaRepository _aulaRepository;
    private readonly IAvaliacaoRepository _avaliacaoRepository;
    private readonly ICurrentUserService _currentUser;

    public DashboardController(
        IPessoaRepository pessoaRepository,
        ITurmaRepository turmaRepository,
        IMensalidadeRepository mensalidadeRepository,
        IAulaRepository aulaRepository,
        IAvaliacaoRepository avaliacaoRepository,
        ICurrentUserService currentUser)
    {
        _pessoaRepository = pessoaRepository;
        _turmaRepository = turmaRepository;
        _mensalidadeRepository = mensalidadeRepository;
        _aulaRepository = aulaRepository;
        _avaliacaoRepository = avaliacaoRepository;
        _currentUser = currentUser;
    }

    public class DashboardResumoDto
    {
        public int TotalAlunos { get; set; }
        public int TurmasAtivas { get; set; }
        public decimal TotalEmAberto { get; set; }
        public int AulasRealizadasMes { get; set; }

        // Professor
        public int MinhasTurmas { get; set; }
        public int TotalAlunosProfessor { get; set; }
        public int AulasHoje { get; set; }
        public int AvaliacoesPendentes { get; set; }
    }

    [HttpGet("resumo")]
    public async Task<IActionResult> GetResumo(CancellationToken ct)
    {
        var resumo = new DashboardResumoDto();

        // Totais gerais
        var pessoas = await _pessoaRepository.GetByTipoAsync(TipoPessoa.Aluno, ct);
        resumo.TotalAlunos = pessoas.Count();

        var turmas = await _turmaRepository.GetAllAsync(ct);
        resumo.TurmasAtivas = turmas.Count(t => t.Ativo);

        var mensalidades = await _mensalidadeRepository.GetAllAsync(ct);
        resumo.TotalEmAberto = mensalidades
            .Where(m => m.Status == StatusMensalidade.EmAberto || m.Status == StatusMensalidade.Vencido)
            .Sum(m => m.Valor.Valor);

        // Aulas realizadas no mês atual
        var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var fimMes = inicioMes.AddMonths(1).AddDays(-1);
        var aulasPeriodo = await _aulaRepository.GetByPeriodoAsync(inicioMes, fimMes, ct);
        resumo.AulasRealizadasMes = aulasPeriodo.Count(a => a.Realizada);

        // Se professor, calcular métricas específicas
        if ((_currentUser.Role == "Professor") && _currentUser.UserId.HasValue)
        {
            var minhas = turmas.Where(t => t.ProfessorId == _currentUser.UserId.Value).ToList();
            resumo.MinhasTurmas = minhas.Count;
            resumo.TotalAlunosProfessor = minhas.Sum(t => t.Matriculas.Count(m => m.Status == StatusMatricula.Ativa));

            var hoje = DateTime.Today;
            var aulasProfessor = aulasPeriodo.Where(a => minhas.Any(t => t.Id == a.TurmaId));
            resumo.AulasHoje = aulasProfessor.Count(a => a.DataAula.Date == hoje);

            // Avaliações pendentes: sem notas
            var avaliacoesProfessor = (await _avaliacaoRepository.GetAllAsync(ct))
                .Where(a => minhas.Any(t => t.Id == a.TurmaId));
            resumo.AvaliacoesPendentes = avaliacoesProfessor.Count(a => !a.Notas.Any());
        }

        return Ok(resumo);
    }
}


