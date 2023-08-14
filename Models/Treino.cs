namespace TreinoSportAPI.Models {
    public class Treino {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public List<Usuario> Alunos { get; set; }
        public DateTime DataCriacao { get; set; }
        public int CodigoCriador { get; set; }
        public List<DiaDaSemana> DatasTreinos { get; set; }
        public DateTime DataVencimento { get; set; }
    }
}
