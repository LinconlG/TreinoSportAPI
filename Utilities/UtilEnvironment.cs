namespace TreinoSportAPI.Utilities {
    public class UtilEnvironment {

        public static string ConnectionString;

        public static void Load(IConfiguration config) {
            ConnectionString = config.GetConnectionString("DataBaseConnection");
        }
    }
}
