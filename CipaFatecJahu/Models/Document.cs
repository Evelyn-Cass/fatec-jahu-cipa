using System.ComponentModel.DataAnnotations;

namespace CipaFatecJahu.Models
{
    public class Document
    {
        [Required]
        public Guid Id { get; set; }
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        public string? Name { get; set; }
        [Display(Name = "Data de criação")]
        public DateTime? DocumentCreationDate { get; set; }
        public DateOnly? ScheduledDate { get; set; }
        [Display(Name = "Publicação da Lei")]
        public DateOnly? LawPublication { get; set; }
        [Display(Name = "Anexar")]
        public string? Attachment { get; set; }
        [Display(Name = "Situação")]
        public string? Status { get; set; }
        public Guid UserId { get; set; }
        public Guid? MandateId { get; set; }
        public Guid MaterialId { get; set; }

    }
}
