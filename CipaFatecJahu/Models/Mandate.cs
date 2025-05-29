using System.ComponentModel.DataAnnotations;

namespace CipaFatecJahu.Models
{
    public class Mandate
    {
        [Required]
        public Guid Id { get; set; }
        [Display(Name = "Ano de início")]
        [Required(ErrorMessage = "O campo Ano de início é obrigatório")]
        public DateOnly StartYear { get; set; }
        [Display(Name = "Ano de término")]
        [Required(ErrorMessage = "O campo Ano de término é obrigatório")]
        public DateOnly TerminationYear { get; set; }
        [Display(Name = "Data de criação")]
        public DateTime DocumentCreationDate { get; set; }
        public Guid UserId { get; set; }
    }
}
