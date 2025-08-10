namespace InstitutoVirtus.API.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using System.Text;

public interface IAuthService
{
    string GenerateToken(Pessoa pessoa);
    Task<string?> RefreshTokenAsync(string token);
    ClaimsPrincipal? ValidateToken(string token);
}

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IPessoaRepository _pessoaRepository;

    public AuthService(IConfiguration configuration, IPessoaRepository pessoaRepository)
    {
        _configuration = configuration;
        _pessoaRepository = pessoaRepository;
    }

    public string GenerateToken(Pessoa pessoa)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, pessoa.Id.ToString()),
            new Claim(ClaimTypes.Name, pessoa.NomeCompleto),
            new Claim(ClaimTypes.Email, pessoa.Email?.Endereco ?? string.Empty),
            new Claim(ClaimTypes.Role, pessoa.TipoPessoa.ToString()),
            new Claim("TipoPessoa", pessoa.TipoPessoa.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string?> RefreshTokenAsync(string token)
    {
        var principal = ValidateToken(token);

        if (principal == null)
            return null;

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
            return null;

        var pessoa = await _pessoaRepository.GetByIdAsync(id);

        if (pessoa == null)
            return null;

        return GenerateToken(pessoa);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // Ignorar expiração para refresh
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = secretKey
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
