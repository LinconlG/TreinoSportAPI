using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services;
using TreinoSportAPI.Services.Interfaces;
using TreinoSportAPI.Tests.Helpers;
using Xunit;

namespace TreinoSportAPI.Tests.Tests.Services {
    public class AuthServiceTests {

        private readonly Mock<ILoginService> _loginServiceMock;
        private readonly IConfiguration _configuration;
        private readonly AuthService _sut;

        public AuthServiceTests() {
            _loginServiceMock = new Mock<ILoginService>();

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> {
                    ["Jwt:Key"] = "TestSecretKey12345678901234567890AB",
                    ["Jwt:Issuer"] = "TestIssuer",
                    ["Jwt:Audience"] = "TestAudience",
                    ["Jwt:ExpireMinutes"] = "30"
                })
                .Build();

            Environment.SetEnvironmentVariable("JWT_SECRET", "TestSecretKey12345678901234567890AB");

            _sut = new AuthService(_configuration, _loginServiceMock.Object);
        }

        [Fact]
        public async Task Autenticar_CredenciaisInvalidas_RetornaNull() {
            // Arrange
            var user = MockBuilders.BuildConta();
            _loginServiceMock.Setup(s => s.Login(user.Email, user.Senha)).ReturnsAsync((Conta)null!);

            // Act
            var resultado = await _sut.Authenticate(user);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Autenticar_CredenciaisValidas_RetornaConta() {
            // Arrange
            var user = MockBuilders.BuildConta();
            var contaRetornada = MockBuilders.BuildConta(codigo: 1, email: "outro@test.com");
            _loginServiceMock.Setup(s => s.Login(user.Email, user.Senha)).ReturnsAsync(contaRetornada);

            // Act
            var resultado = await _sut.Authenticate(user);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Email.Should().Be(user.Email);
        }

        [Fact]
        public void GerarToken_ContaValida_RetornaTokenNaoVazio() {
            // Arrange
            var user = MockBuilders.BuildConta();

            // Act
            var token = _sut.GenerateToken(user);

            // Assert
            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GerarToken_TokenContemRole_CT() {
            // Arrange
            var user = MockBuilders.BuildConta(isCT: true);

            // Act
            var token = _sut.GenerateToken(user);

            // Assert
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var role = jwt.Claims.First(c => c.Type == ClaimTypes.Role || c.Type == "role").Value;
            role.Should().Be("CT");
        }

        [Fact]
        public void GerarToken_TokenContemRole_Aluno() {
            // Arrange
            var user = MockBuilders.BuildConta(isCT: false);

            // Act
            var token = _sut.GenerateToken(user);

            // Assert
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var role = jwt.Claims.First(c => c.Type == ClaimTypes.Role || c.Type == "role").Value;
            role.Should().Be("Aluno");
        }
    }
}
