using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebAppChamadosTI.Models
{
    public class ContaViewModel
    {
        [ValidateNever]
        public Guid? Id { get; set; }

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

        // Métodos

        public ContaViewModel()
        {
            Id = Guid.NewGuid();
        }
    }
}
