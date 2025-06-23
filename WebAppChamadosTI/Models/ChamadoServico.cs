using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppChamadosTI.Models
{
    public class ChamadoServico
    {
        public int Id { get; set; }
        public int Quantidade { get; set; }

        public double ValorTotal { get; set; }

        // FK
        public int ClienteId { get; set; }
        [ForeignKey(nameof(ClienteId))]
        public virtual Cliente Cliente { get; set; }
        // Relacionamento

    }
}
