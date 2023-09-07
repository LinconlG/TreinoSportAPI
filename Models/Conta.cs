namespace TreinoSportAPI.Models {
    public class Conta {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Descricao { get; set; }
        public bool? IsCentroTreinamento { get; set; }
    }
}
