namespace InstitutoVirtus.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using InstitutoVirtus.API.Models;
using InstitutoVirtus.API.Services;
using InstitutoVirtus.Domain.Interfaces.Repositories;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IPessoaRepository _pessoaRepository;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IPessoaRepository pessoaRepository,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _pessoaRepository = pessoaRepository;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var pessoa = await _pessoaRepository.GetByEmailAsync(request.Email);

            if (pessoa == null)
            {
                return Unauthorized(new { message = "Credenciais inválidas" });
            }

            // TODO: Verificar senha quando implementar Identity

            var token = _authService.GenerateToken(pessoa);

            return Ok(new LoginResponse
            {
                Token = token,
                Email = pessoa.Email?.Endereco,
                Nome = pessoa.NomeCompleto,
                Tipo = pessoa.TipoPessoa.ToString(),
                ExpiresIn = 3600
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no login");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var newToken = await _authService.RefreshTokenAsync(request.Token);

            if (string.IsNullOrEmpty(newToken))
            {
                return Unauthorized(new { message = "Token inválido" });
            }

            return Ok(new { token = newToken });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao renovar token");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }
}
