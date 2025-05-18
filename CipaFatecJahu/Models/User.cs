using System.ComponentModel.DataAnnotations;

namespace CipaFatecJahu.Models
{
    public class User
    {
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        [Display(Name = "Nome Completo")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [Display(Name = "Senha")]
        public string? Password { get; set; }
        [Display(Name = "Confirme a senha")]
        [Required(ErrorMessage = "O campo confirme a senha é obrigatório.")]
        public string? ConfirmPassword { get; set; }
        public string? Status { get; set; }
       
    }
}
