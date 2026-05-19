using FluentAssertions;
using Moq;
using TreinoSportAPI.Mappers.Interfaces;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services;
using TreinoSportAPI.Services.Interfaces;
using TreinoSportAPI.Tests.Helpers;
using TreinoSportAPI.Utilities;
using Xunit;

namespace TreinoSportAPI.Tests.Tests.Services {
    public class ContaServiceTests {

        private readonly Mock<IContaMapper> _contaMapperMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly ContaService _sut;

        public ContaServiceTests() {
            _contaMapperMock = new Mock<IContaMapper>();
            _emailServiceMock = new Mock<IEmailService>();
            _sut = new ContaService(_contaMapperMock.Object, _emailServiceMock.Object);
        }

        [Fact]
        public async Task CadastrarUsuario_EmailJaExiste_RetornaTrue() {
            // Arrange
            var conta = MockBuilders.BuildConta();
            _contaMapperMock.Setup(m => m.ChecarEmail(conta.Email)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.CadastrarUsuario(conta);

            // Assert
            resultado.Should().BeTrue();
            _contaMapperMock.Verify(m => m.CadastrarUsuario(It.IsAny<Conta>()), Times.Never);
        }

        [Fact]
        public async Task CadastrarUsuario_Sucesso_ChamaMapperInserir() {
            // Arrange
            var conta = MockBuilders.BuildConta();
            _contaMapperMock.Setup(m => m.ChecarEmail(conta.Email)).ReturnsAsync(false);
            _contaMapperMock.Setup(m => m.CadastrarUsuario(conta)).ReturnsAsync(1);

            // Act
            var resultado = await _sut.CadastrarUsuario(conta);

            // Assert
            resultado.Should().BeFalse();
            _contaMapperMock.Verify(m => m.CadastrarUsuario(conta), Times.Once);
        }

        [Fact]
        public async Task EnviarTokenSenha_EmailNaoExiste_ThrowsAPIException() {
            // Arrange
            _contaMapperMock.Setup(m => m.ChecarEmail("naoexiste@test.com")).ReturnsAsync(false);

            // Act
            var act = async () => await _sut.EnviarTokenSenha("naoexiste@test.com");

            // Assert
            await act.Should().ThrowAsync<APIException>()
                .WithMessage("*email*");
        }

        [Fact]
        public async Task EnviarTokenSenha_EmailExiste_EnviaEmail() {
            // Arrange
            var conta = MockBuilders.BuildConta();
            _contaMapperMock.Setup(m => m.ChecarEmail(conta.Email)).ReturnsAsync(true);
            _contaMapperMock.Setup(m => m.BuscarConta(null, conta.Email)).ReturnsAsync(conta);
            _contaMapperMock.Setup(m => m.InserirToken(conta.Codigo, It.IsAny<string>())).Returns(Task.CompletedTask);
            _emailServiceMock.Setup(m => m.SendPasswordCode(conta.Email, It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var codigoConta = await _sut.EnviarTokenSenha(conta.Email);

            // Assert
            codigoConta.Should().Be(conta.Codigo);
            _emailServiceMock.Verify(m => m.SendPasswordCode(conta.Email, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RedefinirSenha_TokenInvalido_ThrowsAPIException() {
            // Arrange
            _contaMapperMock.Setup(m => m.BuscarTokens(1)).ReturnsAsync(new List<string> { "ABCD" });

            // Act
            var act = async () => await _sut.RedefinirSenha(1, "novaSenha", "XXXX");

            // Assert
            await act.Should().ThrowAsync<APIException>();
        }

        [Fact]
        public async Task RedefinirSenha_TokenValido_AtualizaSenha() {
            // Arrange
            _contaMapperMock.Setup(m => m.BuscarTokens(1)).ReturnsAsync(new List<string> { "ABCD" });
            _contaMapperMock.Setup(m => m.AlterarSenha(1, It.IsAny<string>())).Returns(Task.CompletedTask);
            _contaMapperMock.Setup(m => m.DeletarToken(1)).Returns(Task.CompletedTask);

            // Act
            await _sut.RedefinirSenha(1, "novaSenha", "ABCD");

            // Assert
            _contaMapperMock.Verify(m => m.AlterarSenha(1, It.IsAny<string>()), Times.Once);
            _contaMapperMock.Verify(m => m.DeletarToken(1), Times.Once);
        }
    }
}
