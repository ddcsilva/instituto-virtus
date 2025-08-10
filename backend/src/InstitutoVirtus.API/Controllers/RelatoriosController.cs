using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoVirtus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Coordenacao")]
public class RelatoriosController : ControllerBase
{
    private readonly IMediator _mediator;

    public RelatoriosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("inadimplentes")]
    public async Task<IActionResult> GetInadimplentes([FromQuery] int? mes, [FromQuery] int? ano)
    {
        var query = new ObterInadimplentesQuery
        {
            Mes = mes ?? DateTime.Today.Month,
            Ano = ano ?? DateTime.Today.Year
        };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }

    [HttpGet("boletim/{alunoId}/{turmaId}")]
    [Authorize(Policy = "ResponsavelAluno")]
    public async Task<IActionResult> GetBoletim(Guid alunoId, Guid turmaId)
    {
        var query = new ObterBoletimQuery
        {
            AlunoId = alunoId,
            TurmaId = turmaId
        };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Data);
    }

    [HttpGet("financeiro/mensal")]
    public async Task<IActionResult> GetRelatorioFinanceiroMensal([FromQuery] int mes, [FromQuery] int ano)
    {
        var query = new ObterRelatorioFinanceiroQuery
        {
            Mes = mes,
            Ano = ano
        };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }

    [HttpGet("aproveitamento/turma/{turmaId}")]
    public async Task<IActionResult> GetAproveitamentoTurma(Guid turmaId)
    {
        var query = new ObterAproveitamentoTurmaQuery { TurmaId = turmaId };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }

    [HttpGet("exportar/alunos")]
    public async Task<IActionResult> ExportarAlunos([FromQuery] string formato = "excel")
    {
        var query = new ExportarAlunosQuery { Formato = formato };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        var contentType = formato.ToLower() == "csv"
            ? "text/csv"
            : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        return File(result.Data!, contentType, $"alunos_{DateTime.Now:yyyyMMdd}.{formato}");
    }
}
