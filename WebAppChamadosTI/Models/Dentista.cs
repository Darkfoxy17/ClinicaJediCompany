using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebAppChamadosTI.Models
{
    [Table("Dentistas")]
    public class Dentista
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

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(100, ErrorMessage = "Ultrapassou o máximo permitido")]
        public string Especialidade { get; set; }

        //Fks
        public int UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        [ValidateNever]
        public virtual Usuario Usuario { get; set; }

        //Relacionamento
        [ValidateNever]
        public virtual ICollection<Agendamento> Agendamentos { get; set; }
    }
}
