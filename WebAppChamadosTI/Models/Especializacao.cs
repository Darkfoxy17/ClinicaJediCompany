using System.ComponentModel.DataAnnotations;

namespace WebAppChamadosTI.Models
{
    public class Especializacao
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        public List<EspecializacaoProcedimento> EspecializacaoProcedimentos { get; set; } = new();
    }
}
