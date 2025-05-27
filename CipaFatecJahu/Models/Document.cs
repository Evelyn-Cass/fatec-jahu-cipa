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
        [Display(Name = "Número")]
        public string? Number { get; set; }
        [Display(Name = "Data de criação")]
        public DateTime? DocumentCreationDate { get; set; }
        [Display(Name = "Data da Reunião")]
        public DateOnly? MeetingDate { get; set; }
        [Display(Name = "Publicaçao da Lei")]
        public DateOnly? LawPublication { get; set; }
        [Display(Name = "Anexo")]
        public string? Attachment { get; set; }
        [Display(Name = "Situação")]
        public string? Status { get; set; }
        public Guid UserId { get; set; }
        public Guid? MandateId { get; set; }
        public Guid MaterialId { get; set; }

    }
}
