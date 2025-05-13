using System.ComponentModel.DataAnnotations;

namespace CipaFatecJahu.Models
{
    public class Mandate
    {
        [Required]
        public Guid Id { get; set; }
        [Display(Name = "Ano de início")]
        public DateOnly StartYear { get; set; }
        [Display(Name = "Ano de término")]
        public DateOnly TerminationYear { get; set; }
        [Display(Name = "Data de criação")]
        public DateTime DocumentCreationDate { get; set; }
    }
}
