using InstitutoVirtus.Application.Commands.Matriculas;
using InstitutoVirtus.Application.Queries.Matriculas;
using InstitutoVirtus.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoVirtus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatriculasController : ControllerBase
{
    private readonly IMediator _mediator;

    public MatriculasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> GetAll([FromQuery] StatusMatricula? status = null)
    {
        var query = new ListarMatriculasQuery { Status = status };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "ResponsavelAluno")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new ObterMatriculaQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> RealizarMatricula([FromBody] RealizarMatriculaCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id}/trancar")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> TrancarMatricula(Guid id, [FromBody] TrancarMatriculaCommand command)
    {
        command.MatriculaId = id;
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(new { message = "Matrícula trancada com sucesso" });
    }

    [HttpPut("{id}/reativar")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> ReativarMatricula(Guid id)
    {
        var command = new ReativarMatriculaCommand { MatriculaId = id };
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(new { message = "Matrícula reativada com sucesso" });
    }

    [HttpGet("aluno/{alunoId}")]
    [Authorize(Policy = "ResponsavelAluno")]
    public async Task<IActionResult> GetByAluno(Guid alunoId)
    {
        var query = new ObterMatriculasPorAlunoQuery { AlunoId = alunoId };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }
}
