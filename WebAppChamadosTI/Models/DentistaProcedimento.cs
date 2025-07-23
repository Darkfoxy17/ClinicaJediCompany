using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppChamadosTI.Models
{
    public class DentistaProcedimento
    {
        public int DentistaId { get; set; }
        [ForeignKey("DentistaId")]
        public Dentista Dentista { get; set; }

        public int ProcedimentoId { get; set; }
        [ForeignKey("ProcedimentoId")]
        public Procedimento Procedimento { get; set; }
    }
}
