using Microsoft.AspNetCore.Mvc;
using InstitutoVirtus.API.Models;
using InstitutoVirtus.API.Services;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Exceptions;
using InstitutoVirtus.Domain.ValueObjects;
using InstitutoVirtus.Domain.Entities;

namespace InstitutoVirtus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IPessoaRepository pessoaRepository,
        IUnitOfWork unitOfWork,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _pessoaRepository = pessoaRepository;
        _unitOfWork = unitOfWork;
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
                _logger.LogWarning($"Tentativa de login com email não cadastrado: {request.Email}");
                return Unauthorized(new { message = "Credenciais inválidas" });
            }

            try
            {
                if (!pessoa.VerificarSenha(request.Senha))
                {
                    _logger.LogWarning($"Senha incorreta para: {request.Email}");
                    return Unauthorized(new { message = "Credenciais inválidas" });
                }
            }
            catch (BusinessRuleValidationException ex)
            {
                _logger.LogWarning($"Usuário bloqueado: {request.Email} - {ex.Message}");
                return StatusCode(423, new { message = ex.Message }); // 423 Locked
            }

            var token = _authService.GenerateToken(pessoa);

            _logger.LogInformation($"Login bem-sucedido: {pessoa.Email?.Endereco}");

            return Ok(new LoginResponse
            {
                Token = token,
                RefreshToken = Guid.NewGuid().ToString(), // TODO: Implementar refresh token real
                User = new UserDto
                {
                    Id = pessoa.Id.ToString(),
                    Nome = pessoa.NomeCompleto,
                    Email = pessoa.Email?.Endereco ?? string.Empty,
                    Tipo = pessoa.TipoPessoa.ToString(),
                    PessoaId = pessoa.Id.ToString(),
                    Ativo = pessoa.Ativo
                },
                ExpiresIn = 3600
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no login");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Verificar se email já existe
            var existente = await _pessoaRepository.GetByEmailAsync(request.Email);
            if (existente != null)
                return BadRequest(new { message = "Email já cadastrado" });

            // Criar novo usuário (exemplo com Responsável)
            var telefone = new Telefone(request.Telefone);
            var email = new Email(request.Email);
            Cpf? cpf = !string.IsNullOrWhiteSpace(request.Cpf) ? new Cpf(request.Cpf) : null;

            var pessoa = new Responsavel(
                request.NomeCompleto,
                cpf,
                telefone,
                email,
                request.DataNascimento,
                $"Usuário registrado via API - Senha: {request.Senha}"
            );

            await _pessoaRepository.AddAsync(pessoa);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Novo usuário registrado: {request.Email}");

            return Ok(new { message = "Usuário registrado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar usuário");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var pessoa = await _pessoaRepository.GetByEmailAsync(request.Email);

        if (pessoa == null)
        {
            // Não revelar se o email existe ou não
            return Ok(new { message = "Se o email estiver cadastrado, você receberá instruções para redefinir sua senha" });
        }

        // Gerar token de reset (implementar)
        var resetToken = Guid.NewGuid().ToString();

        // TODO: Salvar token no banco com expiração
        // TODO: Enviar email com link de reset

        _logger.LogInformation($"Solicitação de reset de senha: {request.Email}");

        return Ok(new { message = "Se o email estiver cadastrado, você receberá instruções para redefinir sua senha" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        // TODO: Validar token
        // TODO: Buscar usuário pelo token
        // TODO: Resetar senha

        return Ok(new { message = "Senha redefinida com sucesso" });
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