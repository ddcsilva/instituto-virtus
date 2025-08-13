using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoVirtus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AulasController : ControllerBase
{
    private readonly IAulaRepository _aulaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AulasController(IAulaRepository aulaRepository, IUnitOfWork unitOfWork)
    {
        _aulaRepository = aulaRepository;
        _unitOfWork = unitOfWork;
    }

    public record CriarAulaRequest(Guid TurmaId, DateTime Data, string? Conteudo);

    [HttpPost]
    [Authorize(Policy = "Professor")]
    public async Task<IActionResult> Criar([FromBody] CriarAulaRequest req, CancellationToken ct)
    {
        if (req.TurmaId == Guid.Empty) return BadRequest("TurmaId inválido");

        var existente = await _aulaRepository.GetByTurmaAndDataAsync(req.TurmaId, req.Data, ct);
        if (existente != null) return Ok(new { id = existente.Id });

        var aula = new Aula(req.TurmaId, req.Data, req.Conteudo);
        await _aulaRepository.AddAsync(aula, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(ObterPorId), new { id = aula.Id }, new { id = aula.Id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var aula = await _aulaRepository.GetByIdAsync(id, ct);
        if (aula == null) return NotFound();
        return Ok(new
        {
            aula.Id,
            aula.TurmaId,
            Data = aula.DataAula,
            aula.Conteudo,
            aula.Observacoes,
            aula.Realizada
        });
    }

    [HttpGet("turma/{turmaId}")]
    public async Task<IActionResult> ListarPorTurma(Guid turmaId, [FromQuery] DateTime? inicio, [FromQuery] DateTime? fim, CancellationToken ct)
    {
        if (turmaId == Guid.Empty) return BadRequest("turmaId inválido");
        if (inicio.HasValue && fim.HasValue)
        {
            var aulasPeriodo = await _aulaRepository.GetByPeriodoAsync(inicio.Value, fim.Value, ct);
            return Ok(aulasPeriodo.Where(a => a.TurmaId == turmaId).Select(a => new { a.Id, a.TurmaId, Data = a.DataAula }));
        }

        var aulas = await _aulaRepository.GetByTurmaAsync(turmaId, ct);
        return Ok(aulas.Select(a => new { a.Id, a.TurmaId, Data = a.DataAula }));
    }
}



