using System.ComponentModel.DataAnnotations;

namespace CipaFatecJahu.ViewModel
{
    public class DocumentWithUserMandateMaterialViewModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Nome")]
        public string? Name { get; set; }
        [Display(Name = "Data da Criação")]
        public DateTime? DocumentCreationDate { get; set; }
        [Display(Name = "Anexo")]
        public string? Attachment { get; set; }
        [Display(Name = "Status")]
        public string? Status { get; set; }
        public Guid UserId { get; set; }
        [Display(Name = "Secretário")]
        public string? UserName { get; set; }
        [Display(Name = "Gestão")]
        public string? Mandate { get; set; }
        public string? Material { get; set; }
        [Display(Name = "Data da Reunião")]
        public DateOnly? MeetingDate { get; set; }
        [Display(Name = "Publicação da Lei")]
        public DateOnly? LawPublication { get; set; }
    }
}