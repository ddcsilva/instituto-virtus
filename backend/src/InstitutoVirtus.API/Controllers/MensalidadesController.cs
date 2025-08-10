using InstitutoVirtus.Application.Commands.Mensalidades;
using InstitutoVirtus.Application.Queries.Mensalidades;
using InstitutoVirtus.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoVirtus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MensalidadesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MensalidadesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> GetAll([FromQuery] int? ano, [FromQuery] int? mes, [FromQuery] StatusMensalidade? status)
    {
        var query = new ListarMensalidadesQuery
        {
            Ano = ano,
            Mes = mes,
            Status = status
        };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }

    [HttpGet("responsavel/{responsavelId}")]
    [Authorize(Policy = "ResponsavelAluno")]
    public async Task<IActionResult> GetByResponsavel(Guid responsavelId)
    {
        var query = new ObterMensalidadesEmAbertoQuery { ResponsavelId = responsavelId };
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }

    [HttpGet("vencidas")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> GetVencidas()
    {
        var query = new ObterMensalidadesVencidasQuery();
        var result = await _mediator.Send(query);

        return Ok(result.Data);
    }

    [HttpPut("{id}/pagar")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> RegistrarPagamento(Guid id, [FromBody] RegistrarPagamentoMensalidadeCommand command)
    {
        command.MensalidadeId = id;
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(new { message = "Pagamento registrado com sucesso" });
    }

    [HttpPut("{id}/cancelar-pagamento")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> CancelarPagamento(Guid id)
    {
        var command = new CancelarPagamentoMensalidadeCommand { MensalidadeId = id };
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(new { message = "Pagamento cancelado com sucesso" });
    }
}
