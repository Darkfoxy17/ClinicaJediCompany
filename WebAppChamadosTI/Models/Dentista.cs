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
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(20)]
        public string Telefone { get; set; }

        [Required]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        [Required]
        [MaxLength(100)]
        public string Endereco { get; set; }

        public int UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        [ValidateNever]
        public virtual Usuario Usuario { get; set; }

        [ValidateNever]
        public virtual ICollection<Agendamento> Agendamentos { get; set; }

        // NOVO
        [ValidateNever]
        public virtual ICollection<DentistaProcedimento> DentistaProcedimentos { get; set; }
        public ICollection<Procedimento> Procedimentos { get; set; } = new List<Procedimento>();

        public int EspecializacaoId { get; set; }

        [ForeignKey("EspecializacaoId")]
        public virtual Especializacao Especializacao { get; set; }

    }

}
