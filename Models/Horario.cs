namespace TreinoSportAPI.Models {
    public class Horario {
        public int Codigo { get; set; }
        public DateTime Hora { get; set; }
        public List<Conta> AlunosPresentes { get; set; }
    }
}
