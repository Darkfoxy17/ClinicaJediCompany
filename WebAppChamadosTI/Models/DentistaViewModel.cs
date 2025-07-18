using System.ComponentModel.DataAnnotations;

namespace WebAppChamadosTI.Models
{
    public class DentistaViewModel
    {
        // Dados do Dentista
        [Required]
        public string Nome { get; set; }

        [Required]
        public string Telefone { get; set; }

        [Required]
        public DateTime DataNascimento { get; set; }

        [Required]
        public string Endereco { get; set; }

        [Required]
        public string Especialidade { get; set; }

        // Dados do Usuário
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Senha { get; set; }
    }
}
