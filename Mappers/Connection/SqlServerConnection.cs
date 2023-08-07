using Microsoft.Data.SqlClient;
using System.Data;

namespace TreinoSportAPI.Mappers.Connection {
    public abstract class SqlServerConnection {

        private SqlConnection _connection;

        protected void SetConnectionString(string connection) {
            _connection = new SqlConnection(connection);
        }

        protected DataTableReader Query(string sql, List<(string name, object value)> parameters = null, SqlTransaction transaction = null, bool keepOpen = false, int timeout = 30) {

            try {
                var namedParameters = TuplesToSqlParameters(parameters);
                return ExecuteQuery(sql, namedParameters, transaction, keepOpen, timeout);
            }
            catch (Exception e) {
                if (transaction != null) {
                    transaction.Connection.Close();
                }
                throw new Exception($"{e.Message}; SQLQUERY: {sql}; PARAMETERS: {parameters}");
            }
        }

        protected async Task<int> NonQuery(string sql, List<(string name, object value)> parameters = null, SqlTransaction transaction = null, bool keepOpen = false, int timeout = 30) {

            try {
                var namedParameters = TuplesToSqlParameters(parameters);
                var result = await ExecuteNonQuery(sql, namedParameters, transaction, keepOpen, timeout);
                return result;
            }
            catch (Exception e) {
                if (transaction != null) {
                    transaction.Connection.Close();
                }
                throw new Exception($"{e.Message}; SQLQUERY: {sql}; PARAMETERS: {parameters}");
            }

        }

        private List<SqlParameter> TuplesToSqlParameters(List<(string name, object value)> parameters) {
            List<SqlParameter> namedParameters = null;

            if (parameters != null) {
                namedParameters = new List<SqlParameter>();
                foreach (var (name, value) in parameters) {

                    var nameAux = name;
                    if (!name.StartsWith("@")) {
                        nameAux = "@" + nameAux;
                    }

                    SqlParameter sqlParameter;
                    if (value == null) {
                        sqlParameter = new SqlParameter(nameAux, DBNull.Value);
                    }
                    else {
                        sqlParameter = new SqlParameter(nameAux, value);
                    }
                    namedParameters.Add(sqlParameter);
                }
            }
            return namedParameters;
        }

        private DataTableReader ExecuteQuery(string sql, List<SqlParameter> parameters = null, SqlTransaction transaction = null, bool keepOpen = false, int timeout = 30) {
            var command = CreateCommand(sql, parameters, transaction, timeout);

            var dt = new DataTable();
            dt.Load(command.ExecuteReader());

            if (!keepOpen) {
                command.Connection.Close();
            }

            return dt.CreateDataReader();
        }

        private async Task<int> ExecuteNonQuery(string sql, List<SqlParameter> parameters = null, SqlTransaction transaction = null, bool keepOpen = false, int timeout = 30) {

            var command = CreateCommand(sql, parameters, transaction, timeout);
            var rows = await command.ExecuteNonQueryAsync();

            if (!keepOpen) {
                command.Connection.Close();
            }
            return rows;
        }

        private SqlCommand CreateCommand(string sql, List<SqlParameter> parameters = null, SqlTransaction transaction = null, int timeout = 30) {
            SqlCommand command = _connection.CreateCommand();

            if (_connection.State == ConnectionState.Closed) {
                _connection.Open();
            }

            command.CommandText = sql;
            command.CommandTimeout = timeout;

            if (parameters != null) {
                command.Parameters.AddRange(parameters.ToArray());
            }

            if (transaction != null) {
                command.Transaction = transaction;
            }

            Console.WriteLine(command.CommandText);

            return command;
        }
    }
}
