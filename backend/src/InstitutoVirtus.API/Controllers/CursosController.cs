using InstitutoVirtus.Application.Commands.Cursos;
using InstitutoVirtus.Application.Queries.Cursos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoVirtus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CursosController : ControllerBase
{
    private readonly IMediator _mediator;

    public CursosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> GetAll([FromQuery] string? nome, [FromQuery] bool? ativo, [FromQuery] int page = 0, [FromQuery] int pageSize = 10)
    {
        var query = new ListarCursosQuery { Nome = nome, Ativo = ativo, Page = page, PageSize = pageSize };
        var result = await _mediator.Send(query);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new ObterCursoQuery { Id = id });
        if (!result.IsSuccess) return NotFound(result.Error);
        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> Create([FromBody] CriarCursoCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AtualizarCursoCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Data);
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var result = await _mediator.Send(new ToggleCursoStatusCommand { Id = id });
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Data);
    }
}


