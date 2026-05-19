using TreinoSportAPI.Models;
using TreinoSportAPI.Models.Enums;

namespace TreinoSportAPI.Tests.Helpers {
    /// <summary>
    /// Factory methods for building test objects.
    /// </summary>
    public static class MockBuilders {

        public static Conta BuildConta(int codigo = 1, string email = "test@test.com", string nome = "Test User", bool isCT = false) {
            return new Conta {
                Codigo = codigo,
                Email = email,
                Nome = nome,
                Senha = "senha123",
                IsCentroTreinamento = isCT
            };
        }

        public static Treino BuildTreino(int codigo = 1, int limiteAlunos = 10, int codigoCriador = 1) {
            return new Treino {
                Codigo = codigo,
                Nome = "Treino Teste",
                Descricao = "Descrição teste",
                LimiteAlunos = limiteAlunos,
                DataVencimento = DateTime.Now.AddMonths(1),
                Modalidade = ModalidadeTreino.Funcional,
                Criador = new Conta { Codigo = codigoCriador },
                DatasTreinos = new List<DiaDaSemana>()
            };
        }

        public static Horario BuildHorario(int codigo = 1) {
            return new Horario {
                Codigo = codigo,
                Hora = DateTime.UtcNow,
                AlunosPresentes = new List<Conta>()
            };
        }
    }
}
