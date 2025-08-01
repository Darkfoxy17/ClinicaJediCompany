﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAppChamadosTI.Models
{
    public class DentistaViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Telefone { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        [Required]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "Selecione uma especialização.")]
        [Display(Name = "Especialização")]
        public int EspecializacaoId { get; set; }

        public List<SelectListItem> EspecializacoesDisponiveis { get; set; } = new();

        public List<int> ProcedimentosIds { get; set; } = new();
        public List<SelectListItem> ProcedimentosDisponiveis { get; set; } = new();

        [Required]
        [EmailAddress]
        public string Email { get; set; } // Incluir

        [Required]
        [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
        public string Senha { get; set; } // Incluir

    }
}
