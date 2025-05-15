using System.ComponentModel.DataAnnotations;

namespace CipaFatecJahu.Models
{
    public class User
    {
        [Required]
        [Display(Name = "Nome Completo")]
        public string? Name { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string? Email { get; set; }
        [Required]
        [Display(Name = "Senha")]
        public string? Password { get; set; }
        public string? Status { get; set; }
    }
}
