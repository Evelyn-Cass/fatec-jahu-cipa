using System.ComponentModel.DataAnnotations;

namespace CipaFatecJahu.Models
{
    public class Mandate
    {
        [Required]
        public Guid Id { get; set; }

        [Display(Name = "Início")]
        public DateOnly StartYear { get; set; }

        [Display(Name = "Término")]
        public DateOnly TerminationYear { get; set; }

        [Display(Name = "Data de criação")]
        public DateTime DocumentCreationDate { get; set; }

        public Guid UserId { get; set; }
    }
}
