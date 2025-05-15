using CipaFatecJahu.Models;
using MongoDB.Driver;

namespace CipaFatecJahu.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService()
        {
            var client = new MongoClient(ContextMongodb.ConnectionString);
            _database = client.GetDatabase(ContextMongodb.Database);
        }

        public IMongoCollection<Material> Material => _database.GetCollection<Material>("Material");
    }
}
