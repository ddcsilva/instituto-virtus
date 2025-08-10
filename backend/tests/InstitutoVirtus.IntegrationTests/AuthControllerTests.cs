using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using InstitutoVirtus.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace InstitutoVirtus.IntegrationTests;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
  private readonly WebApplicationFactory<Program> _factory;
  private readonly HttpClient _client;

  public AuthControllerTests(WebApplicationFactory<Program> factory)
  {
    _factory = factory;
    _client = _factory.CreateClient();
  }

  [Fact]
  public async Task Login_WithValidCredentials_ReturnsToken()
  {
    // Arrange
    var request = new LoginRequest
    {
      Email = "admin@institutovirtus.com.br",
      Password = "Admin@123"
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/auth/login", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var content = await response.Content.ReadFromJsonAsync<LoginResponse>();
    content.Should().NotBeNull();
    content!.Token.Should().NotBeEmpty();
    content.Email.Should().Be(request.Email);
  }

  [Fact]
  public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
  {
    // Arrange
    var request = new LoginRequest
    {
      Email = "invalid@email.com",
      Password = "WrongPassword"
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/auth/login", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }
}
