using InstitutoVirtus.Application.Commands.Notas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoVirtus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AvaliacoesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AvaliacoesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("turma/{turmaId}")]
    [Authorize(Policy = "Professor")]
    public async Task<IActionResult> GetByTurma(Guid turmaId)
    {
        var query = new ObterAvaliacoesPorTurmaQuery { TurmaId = turmaId };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Policy = "Professor")]
    public async Task<IActionResult> Create([FromBody] CriarAvaliacaoCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Professor")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new ObterAvaliacaoQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Data);
    }

    [HttpPost("{id}/notas")]
    [Authorize(Policy = "Professor")]
    public async Task<IActionResult> LancarNotas(Guid id, [FromBody] LancarNotasCommand command)
    {
        command.AvaliacaoId = id;
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(new { message = "Notas lançadas com sucesso" });
    }

    [HttpGet("{id}/notas")]
    [Authorize(Policy = "Professor")]
    public async Task<IActionResult> GetNotas(Guid id)
    {
        var query = new ObterNotasPorAvaliacaoQuery { AvaliacaoId = id };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }
}
