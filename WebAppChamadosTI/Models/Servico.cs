using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppChamadosTI.Models
{
    [Table("Servicos")]
    public class Servico
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Codigo")]
        public int Id { get; set; }
        
        [MaxLength(100, ErrorMessage = "Ultrapassou o máximo permitido")]
        [Required(ErrorMessage = " Campo nome é obrigatório")]
        public string Nome { get; set; }
       
        [MaxLength(50, ErrorMessage = "Ultrapassou o máximo permitido")]
        [Required(ErrorMessage = "Campo nome é obrigatório")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        
        [Required(ErrorMessage ="Campo nome é obrigatório")]
        [Display(Name ="Valor")]
        public double Valor { get; set; } // ? permite nulo
    }
}
