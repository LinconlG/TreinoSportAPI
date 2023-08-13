namespace TreinoSportAPI.Mappers {
    public static class Parametros {

        public static List<(string, object)> Parametrizar(List<object> objetos) {
            var parametros = new List<(string, object)>();

            for (int i=0;i<objetos.Count;i++) {
                parametros.Add(($"obj{i}", objetos[i]));
            }
            return parametros;
        }
    }
}
