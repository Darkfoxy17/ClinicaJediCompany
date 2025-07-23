namespace WebAppChamadosTI.Models
{
    public class EspecializacaoProcedimento
    {
        public int EspecializacaoId { get; set; }
        public Especializacao Especializacao { get; set; }

        public int ProcedimentoId { get; set; }
        public Procedimento Procedimento { get; set; }
    }
}
