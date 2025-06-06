using System.ComponentModel.DataAnnotations;

namespace CipaFatecJahu.Models
{
    public class MandateWithUser
    {
        public Guid Id { get; set; }
        [Display(Name = "Ano de início")]
        public DateOnly StartYear { get; set; }
        [Display(Name = "Ano de término")]
        public DateOnly TerminationYear { get; set; }
        [Display(Name = "Data de criação")]
        public DateTime DocumentCreationDate { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
    }
}
