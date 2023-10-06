using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.Mappers.Connection {
    public class BaseMapper : SqlServerConnection {
        public BaseMapper() {
            SetConnectionString(UtilEnvironment.ConnectionString);
        }

        public List<(string, object)> Parametrizar(params object[] objetos) {

            var parametros = new List<(string, object)>();

            for (int i = 0; i < objetos.Length; i++) {
                parametros.Add(($"obj{i}", objetos[i]));
            }
            return parametros;
        }
    }
}
