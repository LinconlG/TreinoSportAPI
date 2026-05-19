using FluentAssertions;
using Moq;
using System.Reflection;
using TreinoSportAPI.BackgroundService;
using TreinoSportAPI.Mappers.Interfaces;
using TreinoSportAPI.Models;
using TreinoSportAPI.Models.DTO;
using Xunit;

namespace TreinoSportAPI.Tests.Tests.BackgroundService {
    public class RenovarAulasBackgroundTests {

        private readonly Mock<ITreinoMapperNoSQL> _mapperMock;
        private readonly RenovarAulasBackground _sut;

        public RenovarAulasBackgroundTests() {
            _mapperMock = new Mock<ITreinoMapperNoSQL>();
            _sut = new RenovarAulasBackground(_mapperMock.Object);
        }

        /// <summary>
        /// Invokes the private ReiniciarPresencas method via reflection.
        /// </summary>
        private async Task InvokeReiniciarPresencas() {
            var method = typeof(RenovarAulasBackground)
                .GetMethod("ReiniciarPresencas", BindingFlags.NonPublic | BindingFlags.Instance)!;
            await (Task)method.Invoke(_sut, null)!;
        }

        [Fact]
        public async Task ReiniciarPresencas_TreinoComDiaDeOntem_ChamaAtualizarDiasHorarios() {
            // Arrange
            var ontem = DateTime.Now.AddDays(-1).DayOfWeek;
            var aluno = new Conta { Codigo = 1, Email = "aluno@test.com" };
            var horario = new Horario { Codigo = 1, Hora = DateTime.UtcNow, AlunosPresentes = new List<Conta> { aluno } };

            var dto = new DiaDaSemanaDTO {
                CodigoTreino = 10,
                DatasTreinos = new List<DiaDaSemana> {
                    new DiaDaSemana {
                        Dia = ontem,
                        Horarios = new List<Horario> { horario }
                    }
                }
            };

            _mapperMock.Setup(m => m.BuscarTodosHorarios()).ReturnsAsync(new List<DiaDaSemanaDTO> { dto });
            _mapperMock.Setup(m => m.AtualizarDiasHorarios(It.IsAny<DiaDaSemanaDTO>())).Returns(Task.CompletedTask);

            // Act
            await InvokeReiniciarPresencas();

            // Assert — AtualizarDiasHorarios should be called once for the matching treino
            _mapperMock.Verify(m => m.AtualizarDiasHorarios(It.Is<DiaDaSemanaDTO>(d => d.CodigoTreino == 10)), Times.Once);

            // The alunos list should have been cleared
            horario.AlunosPresentes.Should().BeEmpty();
        }

        [Fact]
        public async Task ReiniciarPresencas_TreinoComDiaDiferente_NaoChamaAtualizarDiasHorarios() {
            // Arrange — use a day that is NOT yesterday
            var ontem = DateTime.Now.AddDays(-1).DayOfWeek;
            var outroDia = ontem == DayOfWeek.Monday ? DayOfWeek.Tuesday : DayOfWeek.Monday;

            var aluno = new Conta { Codigo = 2, Email = "outro@test.com" };
            var horario = new Horario { Codigo = 2, Hora = DateTime.UtcNow, AlunosPresentes = new List<Conta> { aluno } };

            var dto = new DiaDaSemanaDTO {
                CodigoTreino = 20,
                DatasTreinos = new List<DiaDaSemana> {
                    new DiaDaSemana {
                        Dia = outroDia,
                        Horarios = new List<Horario> { horario }
                    }
                }
            };

            _mapperMock.Setup(m => m.BuscarTodosHorarios()).ReturnsAsync(new List<DiaDaSemanaDTO> { dto });

            // Act
            await InvokeReiniciarPresencas();

            // Assert — AtualizarDiasHorarios should NOT be called
            _mapperMock.Verify(m => m.AtualizarDiasHorarios(It.IsAny<DiaDaSemanaDTO>()), Times.Never);

            // The alunos list should remain untouched
            horario.AlunosPresentes.Should().ContainSingle();
        }

        [Fact]
        public async Task ReiniciarPresencas_MultiplosTreinos_AtualizaApenasOsDeOntem() {
            // Arrange
            var ontem = DateTime.Now.AddDays(-1).DayOfWeek;
            var outroDia = ontem == DayOfWeek.Monday ? DayOfWeek.Tuesday : DayOfWeek.Monday;

            var aluno = new Conta { Codigo = 1, Email = "a@test.com" };

            var dtoOntem = new DiaDaSemanaDTO {
                CodigoTreino = 1,
                DatasTreinos = new List<DiaDaSemana> {
                    new DiaDaSemana { Dia = ontem, Horarios = new List<Horario> {
                        new Horario { Codigo = 1, Hora = DateTime.UtcNow, AlunosPresentes = new List<Conta> { aluno } }
                    }}
                }
            };

            var dtoOutro = new DiaDaSemanaDTO {
                CodigoTreino = 2,
                DatasTreinos = new List<DiaDaSemana> {
                    new DiaDaSemana { Dia = outroDia, Horarios = new List<Horario> {
                        new Horario { Codigo = 2, Hora = DateTime.UtcNow, AlunosPresentes = new List<Conta> { aluno } }
                    }}
                }
            };

            _mapperMock.Setup(m => m.BuscarTodosHorarios()).ReturnsAsync(new List<DiaDaSemanaDTO> { dtoOntem, dtoOutro });
            _mapperMock.Setup(m => m.AtualizarDiasHorarios(It.IsAny<DiaDaSemanaDTO>())).Returns(Task.CompletedTask);

            // Act
            await InvokeReiniciarPresencas();

            // Assert — only the treino with yesterday's day should be updated
            _mapperMock.Verify(m => m.AtualizarDiasHorarios(It.Is<DiaDaSemanaDTO>(d => d.CodigoTreino == 1)), Times.Once);
            _mapperMock.Verify(m => m.AtualizarDiasHorarios(It.Is<DiaDaSemanaDTO>(d => d.CodigoTreino == 2)), Times.Never);
        }
    }
}
