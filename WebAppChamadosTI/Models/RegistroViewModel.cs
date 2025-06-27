using System.ComponentModel.DataAnnotations;

namespace WebAppChamadosTI.Models
{
    public class RegistroViewModel
    {
        public Guid? Id { get; set; }

        //dados usuario
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

        //dados do cliente
        [MaxLength(100, ErrorMessage = "Ultrapassou o máximo permitido")]
        [Required(ErrorMessage = "Campo nome é obrigatório")]
        public string Nome { get; set; }

        //dados do tecnico
        [MaxLength(50, ErrorMessage = "Ultrapassou o máximo permitido")]
        public string? Especialidade { get; set; }

        public RegistroViewModel()
        {
            Id = Guid.NewGuid();
        }
    }
}