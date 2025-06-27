using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebAppChamadosTI.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo e-mail obrigatório")]
        [StringLength(100, ErrorMessage = "Ultrapassou o máximo permitido")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo senha obrigatório")]
        [StringLength(50, ErrorMessage = "Ultrapassou o máximo permitido")]
        [MinLength(8, ErrorMessage = "Senha com mínimo de 8 caracteres")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        public Perfil Perfil { get; set; }

        // Relacionamentos
        [ValidateNever]
        public virtual ICollection<Paciente> Pacientes { get; set; }

        [ValidateNever]
        public virtual ICollection<Dentista> Dentistas { get; set; }
    }

    public enum Perfil
    {
        Paciente = 0,
        Dentista = 1,
        Atendente = 2,
    }
}
