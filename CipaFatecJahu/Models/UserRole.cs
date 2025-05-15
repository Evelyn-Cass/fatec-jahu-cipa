using System.ComponentModel.DataAnnotations;

namespace CipaFatecJahu.Models
{
    public class UserRole
    {
        [Required]
        public string? RoleName { get; set; }
    }
}
