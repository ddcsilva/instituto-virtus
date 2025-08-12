using InstitutoVirtus.Application.Queries.Pessoas;
using InstitutoVirtus.Application.Commands.Pessoas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;

namespace InstitutoVirtus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PessoasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPessoaRepository _pessoaRepository;

    public PessoasController(IMediator mediator, IPessoaRepository pessoaRepository)
    {
        _mediator = mediator;
        _pessoaRepository = pessoaRepository;
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
        [FromQuery] string? cpf,
        [FromQuery] string? telefone,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10)
    {
        var query = new ListarPessoasQuery
        {
            Nome = nome,
            Tipo = tipo,
            Ativo = ativo,
            Cpf = cpf,
            Telefone = telefone,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> Create([FromBody] CriarPessoaCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
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

    // === VÍNCULOS (Responsável <-> Alunos) ===
    [HttpGet("{responsavelId}/vinculos")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> GetVinculos(Guid responsavelId, CancellationToken ct)
    {
        var alunos = await _pessoaRepository.GetAlunosByResponsavelAsync(responsavelId, ct);
        var dto = alunos.Select(a => new
        {
            alunoId = a.Id,
            responsavelId,
            aluno = new { id = a.Id, nome = a.NomeCompleto }
        });
        return Ok(dto);
    }

    public class VincularRequest
    {
        public Guid ResponsavelId { get; set; }
        public IEnumerable<Guid> AlunoIds { get; set; } = Enumerable.Empty<Guid>();
        public string Parentesco { get; set; } = "Responsavel";
        public bool Principal { get; set; } = false;
    }

    [HttpPost("vincular")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> Vincular([FromBody] VincularRequest request, CancellationToken ct)
    {
        foreach (var alunoId in request.AlunoIds)
        {
            var cmd = new VincularResponsavelCommand
            {
                AlunoId = alunoId,
                ResponsavelId = request.ResponsavelId,
                Parentesco = request.Parentesco,
                Principal = request.Principal
            };
            var res = await _mediator.Send(cmd, ct);
            if (!res.IsSuccess) return BadRequest(res.Error);
        }

        return await GetVinculos(request.ResponsavelId, ct);
    }

    [HttpDelete("{responsavelId}/vinculos/{alunoId}")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> Desvincular(Guid responsavelId, Guid alunoId, CancellationToken ct)
    {
        var pessoa = await _pessoaRepository.GetByIdAsync(alunoId, ct);
        if (pessoa == null || pessoa.TipoPessoa != TipoPessoa.Aluno)
            return NotFound("Aluno não encontrado");

        var aluno = (Aluno)pessoa;
        aluno.RemoverResponsavel(responsavelId);
        await _pessoaRepository.UpdateAsync(aluno, ct);
        // Salvar via mediator indireto (não temos IUnitOfWork aqui). Recarregar vínculos após update
        // O SaveChanges é aplicado pelo UnitOfWork no pipeline da aplicação quando usado via commands.
        // Como estamos no controller, simplesmente retornar a lista atualizada após UpdateAsync.

        return await GetVinculos(responsavelId, ct);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AtualizarPessoaCommand command)
    {
        // Usar o ID da rota como fonte da verdade
        command.Id = id;

        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Coordenacao")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var result = await _mediator.Send(new TogglePessoaStatusCommand { Id = id });
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        return Ok(result.Data);
    }
}


