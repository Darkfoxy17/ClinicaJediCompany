using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        [Required]
        public string Endereco { get; set; }

        // Substituído: [Required] public string Especialidade { get; set; }

        // Especialização selecionada
        [Required(ErrorMessage = "Selecione uma especialização.")]
        [Display(Name = "Especialização")]
        public int EspecializacaoId { get; set; }

        // Lista para o dropdown
        public List<SelectListItem> EspecializacoesDisponiveis { get; set; } = new();

        // Dados do Usuário
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
        public string Senha { get; set; }

        // Procedimentos vinculados (futuramente)
        public List<int> ProcedimentosIds { get; set; } = new();
        public List<SelectListItem> ProcedimentosDisponiveis { get; set; } = new();
    }
}
