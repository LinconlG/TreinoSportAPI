using Microsoft.Data.SqlClient;
using System.Data;

namespace TreinoSportAPI.Mappers.Connection {
    public class SqlConnectionFactory {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration) {
            _connectionString = configuration.GetConnectionString("DataBaseConnection")!;
        }

        public IDbConnection CreateConnection() {
            return new SqlConnection(_connectionString);
        }
    }
}
