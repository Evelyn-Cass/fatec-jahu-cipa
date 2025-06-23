using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CipaFatecJahu.ViewModel
{
    public class ContactViewModel
    {
        [DisplayName("Nome")]
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "O campo Assunto é obrigatório.")]
        [DisplayName("Assunto")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "O campo Mensagem é obrigatório.")]
        [DisplayName("Mensagem")]
        public string Text { get; set; }
    }
}
