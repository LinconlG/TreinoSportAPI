using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace TreinoSportAPI.Mappers.NoSQL.Connection {
    public class MongoDBConnection {

        private readonly MongoClient _mongoClient;

        /// <summary>
        /// Inicializa a conexão com o MongoDB e registra o serializador UTC de DateTime.
        /// </summary>
        public MongoDBConnection(IConfiguration configuration) {
            // Register UTC DateTime serializer globally (safe to call multiple times — MongoDB driver ignores duplicate registrations)
            try {
                BsonSerializer.RegisterSerializer(new DateTimeSerializer(DateTimeKind.Utc));
            }
            catch (BsonSerializationException) {
                // Already registered — ignore
            }
            _mongoClient = new MongoClient(configuration.GetConnectionString("MongoDB"));
        }

        /// <summary>
        /// Retorna uma coleção do MongoDB pelo nome do banco e da coleção.
        /// </summary>
        public IMongoCollection<T> GetCollection<T>(string database, string collection) {
            return _mongoClient.GetDatabase(database).GetCollection<T>(collection);
        }
    }
}
