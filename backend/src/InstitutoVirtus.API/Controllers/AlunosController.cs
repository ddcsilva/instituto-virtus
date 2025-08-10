using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using InstitutoVirtus.Application.Commands.Pessoas;
using InstitutoVirtus.Application.Queries.Pessoas;

namespace InstitutoVirtus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlunosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AlunosController> _logger;

    public AlunosController(IMediator mediator, ILogger<AlunosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new ListarAlunosQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "ResponsavelAluno")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new ObterAlunoQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> Create([FromBody] CriarAlunoCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Errors.Any() ? result.Errors : new[] { result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AtualizarAlunoCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID inconsistente");

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new ExcluirAlunoCommand { Id = id };
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpGet("{id}/responsaveis")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> GetResponsaveis(Guid id)
    {
        var query = new ObterResponsaveisDoAlunoQuery { AlunoId = id };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Data);
    }

    [HttpPost("{id}/responsaveis")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> VincularResponsavel(Guid id, [FromBody] VincularResponsavelCommand command)
    {
        command.AlunoId = id;
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(new { message = "Responsável vinculado com sucesso" });
    }
}
