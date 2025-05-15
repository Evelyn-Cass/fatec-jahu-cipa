using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace CipaFatecJahu.Models
{
    [CollectionName("Roles")]
    public class ApplicationRole : MongoIdentityRole
    {
    }
}
