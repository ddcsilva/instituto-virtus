using InstitutoVirtus.Application.Commands.Presencas;
using InstitutoVirtus.Application.Queries.Presencas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoVirtus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PresencasController : ControllerBase
{
    private readonly IMediator _mediator;

    public PresencasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("aula/{aulaId}")]
    [Authorize(Policy = "Professor")]
    public async Task<IActionResult> GetByAula(Guid aulaId)
    {
        var query = new ObterPresencasPorAulaQuery { AulaId = aulaId };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }

    [HttpPost("registrar")]
    [Authorize(Policy = "Professor")]
    public async Task<IActionResult> RegistrarPresencas([FromBody] RegistrarPresencasCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(new { message = "Presenças registradas com sucesso" });
    }

    [HttpGet("aluno/{alunoId}/turma/{turmaId}")]
    [Authorize(Policy = "ResponsavelAluno")]
    public async Task<IActionResult> GetByAlunoTurma(Guid alunoId, Guid turmaId)
    {
        var query = new ObterPresencasDoAlunoQuery
        {
            AlunoId = alunoId,
            TurmaId = turmaId
        };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }

    [HttpGet("relatorio/turma/{turmaId}")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> GetRelatorioFrequencia(Guid turmaId)
    {
        var query = new ObterRelatorioFrequenciaQuery { TurmaId = turmaId };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }
}
