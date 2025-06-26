using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebAppChamadosTI.Models
{
    [Table("Clientes")]
    public class Paciente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(100, ErrorMessage = "Ultrapassou o máximo permitido")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(20, ErrorMessage = "Ultrapassou o máximo permitido")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(15, ErrorMessage = "Ultrapassou o máximo permitido")]
        public string DataNascimento { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(100, ErrorMessage = "Ultrapassou o máximo permitido")]
        public string Endereco { get; set; }

        // Chave estrangeira
        [Display(Name = "Identificação do Usuário")]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        [ValidateNever]
        public virtual Usuario Usuario { get; set; } // ✅ Navegação ao usuário (contém Arquivo, etc.)

        // Relacionamento N:1 com chamados
        [ValidateNever]
        public virtual ICollection<Agendamento> Agendamentos { get; set; }
    }
}
