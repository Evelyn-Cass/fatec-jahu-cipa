using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace CipaFatecJahu.Models
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser
    {
        public string? Name { get; set; }
        public string? Status { get; internal set; }
    }
}
