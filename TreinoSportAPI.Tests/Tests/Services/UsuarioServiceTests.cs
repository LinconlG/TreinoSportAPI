using FluentAssertions;
using Moq;
using TreinoSportAPI.Mappers.Interfaces;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services;
using TreinoSportAPI.Tests.Helpers;
using TreinoSportAPI.Utilities;
using Xunit;

namespace TreinoSportAPI.Tests.Tests.Services {
    public class UsuarioServiceTests {

        private readonly Mock<IContaMapper> _contaMapperMock;
        private readonly UsuarioService _sut;

        public UsuarioServiceTests() {
            _contaMapperMock = new Mock<IContaMapper>();
            var fakeHandler = FakeHttpMessageHandler.ReturnsJson("{}", System.Net.HttpStatusCode.OK);
            var httpClient = new HttpClient(fakeHandler);
            _sut = new UsuarioService(_contaMapperMock.Object, httpClient);
        }

        [Fact]
        public async Task BuscarCTs_SemCoordenadas_SemCep_ThrowsAPIException() {
            // Arrange / Act
            var act = async () => await _sut.BuscarCTs(null, null, null, 10);

            // Assert
            await act.Should().ThrowAsync<APIException>()
                .WithMessage("*coordenadas*");
        }

        [Fact]
        public async Task BuscarCTs_CepInvalido_ThrowsAPIException() {
            // Arrange / Act
            var act = async () => await _sut.BuscarCTs(null, null, "123", 10);

            // Assert
            await act.Should().ThrowAsync<APIException>()
                .WithMessage("*CEP*");
        }

        [Fact]
        public async Task BuscarCTs_ComCoordenadas_RetornaListaOrdenada() {
            // Arrange
            var expectedList = new List<CTResult> {
                new CTResult { Codigo = 1, Nome = "CT Alpha", Descricao = "Desc A", DistanciaKm = 1.5 },
                new CTResult { Codigo = 2, Nome = "CT Beta",  Descricao = "Desc B", DistanciaKm = 3.0 }
            };

            _contaMapperMock
                .Setup(m => m.BuscarCTsPorLocalizacao(-23.5, -46.6, 20))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _sut.BuscarCTs(-23.5, -46.6, null, 20);

            // Assert
            result.Should().BeEquivalentTo(expectedList, options => options.WithStrictOrdering());
            _contaMapperMock.Verify(m => m.BuscarCTsPorLocalizacao(-23.5, -46.6, 20), Times.Once);
        }
    }
}
