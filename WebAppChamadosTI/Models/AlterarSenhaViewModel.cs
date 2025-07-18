using System.ComponentModel.DataAnnotations;

namespace WebAppChamadosTI.Models
{
    public class AlterarSenhaViewModel
    {
        [Required(ErrorMessage = "Informe a senha atual")]
        public string SenhaAtual { get; set; }

        [Required(ErrorMessage = "Informe a nova senha")]
        [MinLength(6, ErrorMessage = "A nova senha deve ter pelo menos 6 caracteres")]
        public string NovaSenha { get; set; }

        [Compare("NovaSenha", ErrorMessage = "A confirmação não confere com a nova senha")]
        public string ConfirmarNovaSenha { get; set; }
    }
}
