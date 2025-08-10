using InstitutoVirtus.Application.Commands.Turmas;
using InstitutoVirtus.Application.Queries.Turmas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoVirtus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TurmasController : ControllerBase
{
    private readonly IMediator _mediator;

    public TurmasController(IMediator mediator)
    {
        _mediator = mediator;
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

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new ObterTurmaQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result.Error);

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
}
