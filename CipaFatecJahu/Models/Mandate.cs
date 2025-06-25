using System.ComponentModel.DataAnnotations;

namespace CipaFatecJahu.Models
{
    public class Mandate
    {
        [Required]
        public Guid Id { get; set; }
        [Display(Name = "Início")]
        [Required(ErrorMessage = "O campo Início é obrigatório")]
        public DateOnly StartYear { get; set; }
        [Display(Name = "Término")]
        [Required(ErrorMessage = "O campo Término é obrigatório")]
        public DateOnly TerminationYear { get; set; }
        [Display(Name = "Data de criação")]
        public DateTime DocumentCreationDate { get; set; }
        public Guid UserId { get; set; }
    }
}
