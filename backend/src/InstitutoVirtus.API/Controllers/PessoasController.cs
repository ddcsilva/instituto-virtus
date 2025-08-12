using InstitutoVirtus.Application.Queries.Pessoas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoVirtus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PessoasController : ControllerBase
{
    private readonly IMediator _mediator;

    public PessoasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista pessoas com filtros e paginação. Os parâmetros de página seguem o front (page 0-based, pageSize).
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> GetAll([
        FromQuery] string? nome,
        [FromQuery] string? tipo,
        [FromQuery] bool? ativo,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10)
    {
        var query = new ListarPessoasQuery
        {
            Nome = nome,
            Tipo = tipo,
            Ativo = ativo,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new ObterPessoaQuery { Id = id });
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Data);
    }
}


