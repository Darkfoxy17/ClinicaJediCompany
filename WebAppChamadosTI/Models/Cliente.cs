using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebAppChamadosTI.Models
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key] // Chave primária
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Código gerado automaticamente
        [Display(Name = "Código")]
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "Ultrapassou o máximo permitido")]
        [Required(ErrorMessage = "Campo nome é obrigatório")]
        public string Nome { get; set; }

        [MaxLength(50, ErrorMessage = "Ultrapassou o máximo permitido")]
        [Required(ErrorMessage = "Campo profissão é obrigatório")]
        [Display(Name = "Profissão")]
        public string Profissao { get; set; }

        [MaxLength(30, ErrorMessage = "Ultrapassou o máximo permitido")]
        public string? Setor { get; set; } // Campo opcional

        // Chave estrangeira
        [Display(Name = "Identificação do Usuário")]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        [ValidateNever]
        public virtual Usuario Usuario { get; set; } // ✅ Navegação ao usuário (contém Arquivo, etc.)

        // Relacionamento N:1 com chamados
        [ValidateNever]
        public virtual ICollection<Chamado> Chamados { get; set; }
    }
}
