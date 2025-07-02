using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebAppChamadosTI.Models
{
    [Table("Atendimentos")]
    public class Agendamento
    {
        //FK
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [DataType(DataType.DateTime, ErrorMessage = "Formato de data inválida")]
        [Display(Name = "Data de solicitação")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(1000, ErrorMessage = "Ultrapassou o máximo permitido")]
        [DataType(DataType.MultilineText)]
        public string Procedimento { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(1000, ErrorMessage = "Ultrapassou o máximo permitido")]
        [DataType(DataType.MultilineText)]
        public string Descricao { get; set; }

        [StringLength(1000, ErrorMessage = "Ultrapassou o máximo permitido")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Status")]
        public string? Status { get; set; }

        //Fks
        //1
        public int PacienteId { get; set; }
        [ForeignKey(nameof(PacienteId))]

        [ValidateNever]
        public virtual Paciente Paciente { get; set; }
        //1
        public int DentistaId { get; set; }
        [ForeignKey(nameof(DentistaId))]

        [ValidateNever]
        public virtual Dentista Dentista { get; set; }
    }
}
