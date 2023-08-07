using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.Mappers.Connection {
    public class BaseMapper : SqlServerConnection {
        public BaseMapper() {
            SetConnectionString(UtilEnvironment.ConnectionString);
        }
    }
}
