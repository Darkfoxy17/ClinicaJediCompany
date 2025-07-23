using System.ComponentModel.DataAnnotations;

namespace WebAppChamadosTI.Models
{
    public class Procedimento
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        public ICollection<Agendamento> Agendamentos { get; set; }
        public ICollection<DentistaProcedimento> DentistaProcedimentos { get; set; }
        public ICollection<EspecializacaoProcedimento> EspecializacaoProcedimentos { get; set; }
    }
}
