using System.ComponentModel.DataAnnotations;

namespace CipaFatecJahu.Models
{
    public class Material
    {
        [Required]
        public Guid Id { get; set; }
        [Display(Name = "Descritivo")]
        public string? Description { get; set; }

    }
}
