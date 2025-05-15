using AspNetCore.Identity.MongoDbCore.Infrastructure;
using CipaFatecJahu.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CipaFatecJahu.Services
{
    public class MaterialService
    {
        private readonly IMongoCollection<Material> _materialCollection;

        public MaterialService(IOptions<MongoDbSettings> mongoSettings)
        {
            var client = new MongoClient(mongoSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
            _materialCollection = database.GetCollection<Material>("Material");
        }

        public List<Material> GetAll() =>
            _materialCollection.Find(material => true).ToList();

        public Material GetById(Guid id) =>
            _materialCollection.Find(material => material.Id == id).FirstOrDefault();

        public void Create(Material material) =>
            _materialCollection.InsertOne(material);

        public void Update(Guid id, Material updatedMaterial) =>
            _materialCollection.ReplaceOne(material => material.Id == id, updatedMaterial);

        public void Delete(Guid id) =>
            _materialCollection.DeleteOne(material => material.Id == id);
    }
}
