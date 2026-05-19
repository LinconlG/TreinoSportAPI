namespace TreinoSportAPI.Models {
    /// <summary>
    /// Resultado de busca de Centro de Treinamento por localização.
    /// </summary>
    public class CTResult {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public double DistanciaKm { get; set; }
        public List<string> Modalidades { get; set; } = new();
    }
}
