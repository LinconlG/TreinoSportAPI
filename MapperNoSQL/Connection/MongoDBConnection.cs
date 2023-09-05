using MongoDB.Driver;

namespace TreinoSportAPI.MapperNoSQL.Connection {
    public class MongoDBConnection {

        private readonly MongoClient _mongoClient;

        public MongoDBConnection(IConfiguration configuration) { 
            _mongoClient = new MongoClient(configuration.GetConnectionString("MongoDB"));
        }

        public IMongoCollection<T> GetCollection<T>(string database, string collection) {
            return _mongoClient.GetDatabase(database).GetCollection<T>(collection);
        }
    }
}
