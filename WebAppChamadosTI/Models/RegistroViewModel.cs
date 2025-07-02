using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppChamadosTI.Models
{
    public class RegistroViewModel
    {
        public Guid? Id { get; set; }

        // Dados do usuário
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

        // Dados do paciente
        [Required(ErrorMessage = "Campo nome é obrigatório")]
        [MaxLength(100, ErrorMessage = "Ultrapassou o máximo permitido")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo telefone é obrigatório")]
        [MaxLength(20, ErrorMessage = "Ultrapassou o máximo permitido")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "Campo data de nascimento é obrigatório")]
        [MaxLength(15, ErrorMessage = "Ultrapassou o máximo permitido")]
        [Display(Name = "Data de nascimento")]
        public string DataNascimento { get; set; }

        [Required(ErrorMessage = "Campo endereço é obrigatório")]
        [MaxLength(100, ErrorMessage = "Ultrapassou o máximo permitido")]
        public string Endereco { get; set; }

        public RegistroViewModel()
        {
            Id = Guid.NewGuid();
        }
    }
}
