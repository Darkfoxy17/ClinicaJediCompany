using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebAppChamadosTI.Models
{
    [Table("Chamados")]
    public class Chamado
    {
        //FK
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo data de solicitação é obrigatório")]
        [DataType(DataType.DateTime, ErrorMessage = "Formato de data inválida")]
        [Display(Name = "Data de solicitação")]
        public DateTime DataSolicitacao { get; set; }

        [Required(ErrorMessage = "Campo problema obrigatório")]
        [StringLength(1000, ErrorMessage = "Ultrapassou o máximo permitido")]
        [DataType(DataType.MultilineText)]
        public string Problema { get; set; }

        [StringLength(1000, ErrorMessage = "Ultrapassou o máximo permitido")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ocorrência")]
        public string? Ocorrencia { get; set; }

        [Display(Name = "Concluído")]
        public bool Concluido { get; set; }

        [Range(0, 100000.00, ErrorMessage ="Ultrapassou o máximo permitido")]
        [DataType(DataType.Currency)]
        [Display(Name = "Valor Total")]
        public double ValorTotal { get; set; }

        //Fks
        //1
        public int ClienteId { get; set; }
        [ForeignKey(nameof(ClienteId))]

        [ValidateNever]
        public virtual Cliente Cliente { get; set; }
        //1
        public int TecnicoId { get; set; }
        [ForeignKey(nameof(TecnicoId))]

        [ValidateNever]
        public virtual Tecnico Tecnico { get; set; }
    }
}
