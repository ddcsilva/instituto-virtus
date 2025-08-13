using InstitutoVirtus.Application.Commands.Turmas;
using InstitutoVirtus.Application.Queries.Turmas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InstitutoVirtus.Domain.Enums;
using System.Globalization;

namespace InstitutoVirtus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TurmasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TurmasController> _logger;

    public TurmasController(IMediator mediator, ILogger<TurmasController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? anoLetivo, [FromQuery] int? periodo)
    {
        var query = new ListarTurmasQuery
        {
            AnoLetivo = anoLetivo ?? DateTime.Today.Year,
            Periodo = periodo
        };
        var result = await _mediator.Send(query);

        try
        {
            var count = result.Data?.Count ?? 0;
            _logger.LogInformation("[TurmasController.GetAll] AnoLetivo={Ano}, Periodo={Periodo}, Total={Total}", query.AnoLetivo, query.Periodo, count);
            if (count > 0)
            {
                var t = result.Data![0];
                _logger.LogInformation("[Turma 0] Nome={Nome}, CursoNome={Curso}, ProfessorNome={Prof}, Dia={Dia}, HIni={Ini}, HFim={Fim}, Ativo={Ativo}", t.Nome, t.CursoNome, t.ProfessorNome, t.DiaSemana, t.HorarioInicio, t.HorarioFim, t.Ativo);
            }
        }
        catch { /* no-op */ }

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new ObterTurmaQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        try
        {
            var t = result.Data!;
            _logger.LogInformation("[TurmasController.GetById] Id={Id}, Nome={Nome}, CursoNome={Curso}, ProfessorNome={Prof}, Dia={Dia}, HIni={Ini}, HFim={Fim}, Ativo={Ativo}", id, t.Nome, t.CursoNome, t.ProfessorNome, t.DiaSemana, t.HorarioInicio, t.HorarioFim, t.Ativo);
        }
        catch { /* no-op */ }

        return Ok(result.Data);
    }

    [HttpGet("disponiveis")]
    public async Task<IActionResult> GetDisponiveis()
    {
        var query = new ObterTurmasComVagasQuery();
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> Create([FromBody] CriarTurmaCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AtualizarTurmaCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID inconsistente");

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }

    [HttpGet("{id}/alunos")]
    [Authorize(Policy = "Professor")]
    public async Task<IActionResult> GetAlunos(Guid id)
    {
        var query = new ObterAlunosDaTurmaQuery { TurmaId = id };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }

    [HttpGet("{id}/grade-horarios")]
    public async Task<IActionResult> GetGradeHorarios(Guid id)
    {
        var query = new ObterGradeHorariosQuery { TurmaId = id };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var cmd = new ToggleTurmaStatusCommand { Id = id };
        var result = await _mediator.Send(cmd);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Data);
    }

    [HttpGet("confere-conflito")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> ConfereConflito([FromQuery] Guid professorId, [FromQuery] string diaSemana, [FromQuery] string horaInicio, CancellationToken ct)
    {
        try
        {
            if (professorId == Guid.Empty) return BadRequest("professorId inválido");
            if (string.IsNullOrWhiteSpace(diaSemana)) return BadRequest("diaSemana é obrigatório");
            if (string.IsNullOrWhiteSpace(horaInicio)) return BadRequest("horaInicio é obrigatório");

            // Normalizar nome do dia para enum
            var dia = Enum.Parse<DiaSemana>(diaSemana, true);
            // Aceitar HH:mm ou HH:mm:ss
            if (horaInicio.Length == 5) horaInicio += ":00";
            if (!TimeSpan.TryParseExact(horaInicio, "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out var ts))
                return BadRequest("horaInicio inválido");

            var existe = await _turmaRepository.ExisteConflitoHorarioAsync(professorId, dia, ts, ct);
            _logger.LogInformation("[ConflitoHorario] Prof={Prof} Dia={Dia} Hora={Hora} -> {Existe}", professorId, dia, ts, existe);
            return Ok(new { conflito = existe });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao conferir conflito de horário");
            return StatusCode(500, "Erro ao conferir conflito");
        }
    }
}
