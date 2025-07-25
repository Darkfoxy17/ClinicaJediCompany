using System.ComponentModel.DataAnnotations;

namespace WebAppChamadosTI.Models
{
    public class AlterarSenhaViewModel
    {
        [Required(ErrorMessage = "Informe a senha atual")]
        [Display(Name = "Senha atual")]
        public string SenhaAtual { get; set; }

        [Required(ErrorMessage = "Informe a nova senha")]
        [MinLength(8, ErrorMessage = "A nova senha deve ter pelo menos 8 caracteres")]
        [Display(Name = "Nova senha")]
        public string NovaSenha { get; set; }

        [Compare("NovaSenha", ErrorMessage = "A confirmação não confere com a nova senha")]
        [Display(Name = "Confirme nova senha")]
        public string ConfirmarNovaSenha { get; set; }
    }
}
