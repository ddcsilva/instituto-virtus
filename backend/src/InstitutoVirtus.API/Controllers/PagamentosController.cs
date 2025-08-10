using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoVirtus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PagamentosController : ControllerBase
{
    private readonly IMediator _mediator;

    public PagamentosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
    {
        var query = new ListarPagamentosQuery
        {
            DataInicio = inicio ?? DateTime.Today.AddMonths(-1),
            DataFim = fim ?? DateTime.Today
        };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new ObterPagamentoQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> RegistrarPagamento([FromBody] RegistrarPagamentoCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet("responsavel/{responsavelId}")]
    [Authorize(Policy = "ResponsavelAluno")]
    public async Task<IActionResult> GetByResponsavel(Guid responsavelId)
    {
        var query = new ObterPagamentosPorResponsavelQuery { ResponsavelId = responsavelId };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }

    [HttpPost("processar-arquivo")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> ProcessarArquivo(IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
            return BadRequest("Arquivo não fornecido");

        var command = new ProcessarArquivoPagamentoCommand
        {
            Arquivo = arquivo.OpenReadStream(),
            NomeArquivo = arquivo.FileName
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }
}
