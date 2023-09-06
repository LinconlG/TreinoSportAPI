using TreinoSportAPI.Models.Enums;

namespace TreinoSportAPI.Models {
    public class Treino {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public List<Conta> Alunos { get; set; }
        public DateTime DataCriacao { get; set; }
        public Conta Criador { get; set; }
        public List<DiaDaSemana> DatasTreinos { get; set; }
        public DateTime DataVencimento { get; set; }
        public ModalidadeTreino Modalidade { get; set; }
    }
}
