using FluentAssertions;
using Moq;
using TreinoSportAPI.Mappers.Interfaces;
using TreinoSportAPI.Models;
using TreinoSportAPI.Models.DTO;
using TreinoSportAPI.Services;
using TreinoSportAPI.Tests.Helpers;
using TreinoSportAPI.Utilities;
using Xunit;

namespace TreinoSportAPI.Tests.Tests.Services {
    public class TreinoServiceTests {

        private readonly Mock<ITreinoMapper> _treinoMapperMock;
        private readonly Mock<ITreinoMapperNoSQL> _treinoMapperNoSQLMock;
        private readonly Mock<IContaMapper> _contaMapperMock;
        private readonly TreinoService _sut;

        public TreinoServiceTests() {
            _treinoMapperMock = new Mock<ITreinoMapper>();
            _treinoMapperNoSQLMock = new Mock<ITreinoMapperNoSQL>();
            _contaMapperMock = new Mock<IContaMapper>();
            _sut = new TreinoService(_treinoMapperMock.Object, _treinoMapperNoSQLMock.Object, _contaMapperMock.Object);
        }

        [Fact]
        public async Task InserirTreino_MapperRetornaZero_ThrowsException() {
            // Arrange
            var treino = MockBuilders.BuildTreino();
            _treinoMapperMock.Setup(m => m.InserirTreino(treino)).ReturnsAsync(0);

            // Act
            var act = async () => await _sut.InserirTreino(treino);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro ao inserir treino");
        }

        [Fact]
        public async Task InserirTreino_Sucesso_ChamaMapperEInserirHorarios() {
            // Arrange
            var treino = MockBuilders.BuildTreino();
            treino.DatasTreinos = new List<DiaDaSemana>();
            _treinoMapperMock.Setup(m => m.InserirTreino(treino)).ReturnsAsync(42);
            _treinoMapperNoSQLMock.Setup(m => m.InserirHorarios(It.IsAny<DiaDaSemanaDTO>())).Returns(Task.CompletedTask);

            // Act
            await _sut.InserirTreino(treino);

            // Assert
            _treinoMapperMock.Verify(m => m.InserirTreino(treino), Times.Once);
            _treinoMapperNoSQLMock.Verify(m => m.InserirHorarios(It.Is<DiaDaSemanaDTO>(d => d.CodigoTreino == 42)), Times.Once);
        }

        [Fact]
        public async Task AdicionarAluno_EmailNaoExiste_ThrowsAPIException() {
            // Arrange
            _contaMapperMock.Setup(m => m.ChecarEmail("naoexiste@test.com")).ReturnsAsync(false);

            // Act
            var act = async () => await _sut.AdicionarAluno(1, "naoexiste@test.com");

            // Assert
            await act.Should().ThrowAsync<APIException>()
                .WithMessage("Email não existe.");
        }

        [Fact]
        public async Task AdicionarAluno_EmailExiste_RetornaConta() {
            // Arrange
            var conta = MockBuilders.BuildConta();
            _contaMapperMock.Setup(m => m.ChecarEmail(conta.Email)).ReturnsAsync(true);
            _treinoMapperMock.Setup(m => m.AdicionarAluno(1, conta.Email)).ReturnsAsync(conta.Codigo);
            _contaMapperMock.Setup(m => m.BuscarConta(conta.Codigo, null)).ReturnsAsync(conta);

            // Act
            var resultado = await _sut.AdicionarAluno(1, conta.Email);

            // Assert
            resultado.Should().BeEquivalentTo(conta);
            _treinoMapperMock.Verify(m => m.AdicionarAluno(1, conta.Email), Times.Once);
        }

        [Fact]
        public async Task BuscarAlunosPresentes_HorarioInexistente_ThrowsAPIException() {
            // Arrange
            var dto = new DiaDaSemanaDTO {
                CodigoTreino = 1,
                DatasTreinos = new List<DiaDaSemana> {
                    new DiaDaSemana {
                        Dia = DayOfWeek.Monday,
                        Horarios = new List<Horario> {
                            MockBuilders.BuildHorario(codigo: 99)
                        }
                    }
                }
            };
            _treinoMapperNoSQLMock.Setup(m => m.BuscarAlunosPresentes(1)).ReturnsAsync(dto);

            // Act — codigoDia=2 (Tuesday) does not exist in the DTO
            var act = async () => await _sut.BuscarAlunosPresentes(1, 2, 99);

            // Assert
            await act.Should().ThrowAsync<APIException>();
        }

        [Fact]
        public async Task BuscarAlunosPresentes_HorarioExiste_RetornaAlunos() {
            // Arrange
            var aluno = MockBuilders.BuildConta();
            var horario = MockBuilders.BuildHorario(codigo: 5);
            horario.AlunosPresentes.Add(aluno);

            var dto = new DiaDaSemanaDTO {
                CodigoTreino = 1,
                DatasTreinos = new List<DiaDaSemana> {
                    new DiaDaSemana {
                        Dia = DayOfWeek.Monday,   // DayOfWeek.Monday == 1
                        Horarios = new List<Horario> { horario }
                    }
                }
            };
            _treinoMapperNoSQLMock.Setup(m => m.BuscarAlunosPresentes(1)).ReturnsAsync(dto);

            // Act — codigoDia=1 matches DayOfWeek.Monday, codigoHorario=5 matches horario
            var resultado = await _sut.BuscarAlunosPresentes(1, 1, 5);

            // Assert
            resultado.Should().ContainSingle()
                .Which.Codigo.Should().Be(aluno.Codigo);
        }

        [Fact]
        public async Task DeletarTreino_Sucesso_ChamaAmbosMappers() {
            // Arrange
            _treinoMapperMock.Setup(m => m.DeletarAlunosTreino(1)).Returns(Task.CompletedTask);
            _treinoMapperMock.Setup(m => m.DeletarTreino(1)).Returns(Task.CompletedTask);

            // Act
            await _sut.DeletarTreino(1);

            // Assert
            _treinoMapperMock.Verify(m => m.DeletarAlunosTreino(1), Times.Once);
            _treinoMapperMock.Verify(m => m.DeletarTreino(1), Times.Once);
        }
    }
}
